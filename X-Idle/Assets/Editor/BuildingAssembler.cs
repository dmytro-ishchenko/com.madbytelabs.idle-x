using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadByteLabs.BuildingTools
{
    internal static class BuildingAssembler
    {
        private const string AssembleOperation = "Assemble Building Preview";

        // JsonUtility populates the recipe fields at runtime.
#pragma warning disable CS0649
        [Serializable]
        private sealed class Recipe
        {
            public string name;
            public Piece[] entries;
        }

        [Serializable]
        private sealed class Piece
        {
            public int id;
            public string group;
            public string name;
            public string prefab;
            public string guid;
            public float[] localPosition;
            public float[] localEulerAngles;
            public float[] localScale;
        }

#pragma warning restore CS0649

        [MenuItem("Tools/A.F.T.E.R./Assemble Building Preview")]
        private static void Assemble()
        {
            string path = EditorUtility.OpenFilePanel("Choose Building Recipe", "", "json");
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                var recipe = JsonUtility.FromJson<Recipe>(File.ReadAllText(path));
                Dictionary<string, GameObject> prefabs = ResolvePrefabs(recipe);
                Build(recipe, prefabs);
            }
            catch (Exception exception)
            {
                EditorUtility.DisplayDialog("Building Preview", exception.Message, "OK");
            }
        }

        private static Dictionary<string, GameObject> ResolvePrefabs(Recipe recipe)
        {
            if (recipe == null || string.IsNullOrWhiteSpace(recipe.name) ||
                recipe.entries == null || recipe.entries.Length == 0)
                throw new InvalidDataException("The recipe must contain a building name and at least one entry.");

            var prefabs = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
            var ids = new HashSet<int>();
            foreach (Piece piece in recipe.entries)
            {
                if (piece == null || piece.id < 1 || !ids.Add(piece.id) ||
                    string.IsNullOrWhiteSpace(piece.group) || string.IsNullOrWhiteSpace(piece.guid))
                    throw new InvalidDataException("Each entry must have a unique positive ID, a group, and a prefab GUID.");

                ReadVector(piece.localPosition, nameof(piece.localPosition), piece.id);
                ReadVector(piece.localEulerAngles, nameof(piece.localEulerAngles), piece.id);
                Vector3 scale = ReadVector(piece.localScale, nameof(piece.localScale), piece.id);
                if (scale.x <= 0 || scale.y <= 0 || scale.z <= 0)
                    throw new InvalidDataException($"Scale must be positive for entry {piece.id}.");

                if (prefabs.ContainsKey(piece.guid))
                    continue;

                string assetPath = AssetDatabase.GUIDToAssetPath(piece.guid);
                GameObject prefab = string.IsNullOrEmpty(assetPath)
                    ? null
                    : AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (!prefab || !PrefabUtility.IsPartOfPrefabAsset(prefab))
                    throw new InvalidDataException($"Entry {piece.id}: missing prefab {piece.prefab} ({piece.guid}).");

                prefabs.Add(piece.guid, prefab);
            }

            return prefabs;
        }

        private static void Build(Recipe recipe, Dictionary<string, GameObject> prefabs)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded || PrefabStageUtility.GetCurrentPrefabStage() != null)
                throw new InvalidOperationException("Open a normal scene to assemble the preview.");

            Undo.IncrementCurrentGroup();
            int scope = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(AssembleOperation);

            GameObject root;
            try
            {
                string buildingName = EnglishNameOrDefault(recipe.name, "Building");
                root = new GameObject($"{buildingName} Preview");
                Undo.RegisterCreatedObjectUndo(root, AssembleOperation);
                var groups = new Dictionary<string, Transform>(StringComparer.Ordinal);

                foreach (Piece piece in recipe.entries)
                {
                    string groupKey = piece.group.Trim();
                    if (!groups.TryGetValue(groupKey, out Transform parent))
                    {
                        var groupObject = new GameObject(EnglishNameOrDefault(groupKey, $"Group {groups.Count + 1:00}"));
                        Undo.RegisterCreatedObjectUndo(groupObject, AssembleOperation);
                        Undo.SetTransformParent(groupObject.transform, root.transform, AssembleOperation);
                        parent = groupObject.transform;
                        groups.Add(groupKey, parent);
                    }

                    AddPiece(piece, prefabs[piece.guid], parent);
                }
            }
            catch
            {
                Undo.RevertAllDownToGroup(scope);
                throw;
            }

            Undo.CollapseUndoOperations(scope);
            Selection.activeGameObject = root;
            SceneView.lastActiveSceneView?.FrameSelected();
        }

        private static void AddPiece(Piece piece, GameObject prefab, Transform parent)
        {
            var instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (!instance)
                throw new InvalidOperationException($"Could not instantiate entry {piece.id} ({prefab.name}).");

            Undo.RegisterCreatedObjectUndo(instance, AssembleOperation);
            Transform instanceTransform = instance.transform;
            Undo.RegisterCompleteObjectUndo(instanceTransform, AssembleOperation);
            instanceTransform.localPosition = ReadVector(piece.localPosition, nameof(piece.localPosition), piece.id);
            instanceTransform.localRotation = Quaternion.Euler(ReadVector(piece.localEulerAngles, nameof(piece.localEulerAngles), piece.id));
            instanceTransform.localScale = ReadVector(piece.localScale, nameof(piece.localScale), piece.id);
            PrefabUtility.RecordPrefabInstancePropertyModifications(instanceTransform);

            string prefabLabel = EnglishNameOrDefault(prefab.name, "Prefab");
            string recipeLabel = EnglishNameOrDefault(piece.prefab, prefabLabel);
            string label = EnglishNameOrDefault(piece.name, recipeLabel);
            RenamePrefabObject(instance, $"{piece.id:000} {label}");

            int number = 0;
            foreach (Transform child in instance.GetComponentsInChildren<Transform>(includeInactive: true))
            {
                if (child == instanceTransform)
                    continue;

                number++;
                RenamePrefabObject(child.gameObject, EnglishNameOrDefault(child.name, $"Part {number:000}"));
            }
        }

        private static void RenamePrefabObject(GameObject instance, string name)
        {
            if (instance.name == name)
                return;

            Undo.RegisterCompleteObjectUndo(instance, AssembleOperation);
            instance.name = name;
            PrefabUtility.RecordPrefabInstancePropertyModifications(instance);
        }

        private static string EnglishNameOrDefault(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
                return fallback;

            string name = value.Trim();
            bool containsLetter = false;
            foreach (char character in name)
            {
                if (character < ' ' || character > '~')
                    return fallback;

                if ((character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z'))
                    containsLetter = true;
            }

            return containsLetter ? name : fallback;
        }

        private static Vector3 ReadVector(float[] value, string field, int id)
        {
            if (value == null || value.Length != 3)
                throw new InvalidDataException($"Entry {id}: {field} must have three numbers.");

            foreach (float component in value)
            {
                if (float.IsInfinity(component) || float.IsNaN(component))
                    throw new InvalidDataException($"Entry {id}: {field} must contain finite numbers.");
            }

            return new Vector3(value[0], value[1], value[2]);
        }
    }
}
