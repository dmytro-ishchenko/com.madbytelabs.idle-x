using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MadByteLabs.BuildingTools
{
    internal static class PrefabRecipeExporter
    {
        private const string Menu = "Tools/A.F.T.E.R./Export Selected Prefab to JSON";
        private const string Title = "Export Prefab to JSON";
        private const float OrthogonalityTolerance = 0.0001f;

        [Serializable]
        private sealed class Recipe
        {
            public string name;
            public string units = "meters";
            public string sourcePrefab;
            public List<Entry> entries = new List<Entry>();
        }

        [Serializable]
        private sealed class Entry
        {
            public int id;
            public string group;
            public string name;
            public string prefab;
            public string guid;
            public string assetPath;
            public float[] localPosition;
            public float[] localEulerAngles;
            public float[] localScale;
        }

        [MenuItem(Menu, true)]
        private static bool CanExport()
        {
            return Selection.objects.Length == 1 &&
                   Selection.activeObject is GameObject selected &&
                   AssetDatabase.IsMainAsset(selected) &&
                   PrefabUtility.IsPartOfPrefabAsset(selected);
        }

        [MenuItem(Menu)]
        private static void Export()
        {
            var prefab = (GameObject)Selection.activeObject;
            string assetPath = AssetDatabase.GetAssetPath(prefab);
            var warnings = new List<string>();

            try
            {
                Recipe recipe = ReadPrefab(prefab, assetPath, warnings);
                if (warnings.Count > 0)
                {
                    int visibleCount = Math.Min(warnings.Count, 5);
                    string details = string.Join("\n", warnings.GetRange(0, visibleCount));
                    if (warnings.Count > visibleCount)
                        details += $"\n… and {warnings.Count - visibleCount} more. See the Console.";

                    foreach (string warning in warnings)
                        Debug.LogWarning($"[{Title}] {warning}", prefab);

                    bool proceed = EditorUtility.DisplayDialog(Title,
                        "The assembly format cannot preserve some prefab-specific data.\n\n" + details,
                        "Export Preview Recipe", "Cancel");
                    if (!proceed)
                        return;
                }

                string jsonPath = EditorUtility.SaveFilePanel(Title, "", prefab.name, "json");
                if (string.IsNullOrEmpty(jsonPath))
                    return;

                File.WriteAllText(jsonPath, JsonUtility.ToJson(recipe, true) + "\n");
                Debug.Log($"[{Title}] Saved {recipe.entries.Count} entries to {jsonPath}", prefab);
            }
            catch (Exception exception)
            {
                EditorUtility.DisplayDialog(Title, exception.Message, "OK");
                Debug.LogException(exception, prefab);
            }
        }

        private static Recipe ReadPrefab(GameObject prefab, string assetPath, List<string> warnings)
        {
            var recipe = new Recipe { name = prefab.name, sourcePrefab = assetPath };
            if (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Model)
            {
                recipe.entries.Add(MakeEntry(1, "Root", prefab.name, prefab, assetPath, Matrix4x4.identity));
                return recipe;
            }

            GameObject contents = PrefabUtility.LoadPrefabContents(assetPath);
            try
            {
                // When the prefab is a single authored asset, it is already a reusable piece.
                // A variant without nested pieces must keep its own GUID to retain its overrides.
                if (!HasNestedInstances(contents.transform))
                {
                    recipe.entries.Add(MakeEntry(1, "Root", prefab.name, prefab, assetPath, Matrix4x4.identity));
                    return recipe;
                }

                RecordWrapperWarning(contents.transform, "", warnings);
                foreach (Transform child in contents.transform)
                {
                    ReadNode(child, Matrix4x4.identity, "", contents.activeSelf, recipe, warnings);
                }

                if (recipe.entries.Count == 0)
                    throw new InvalidDataException("The selected prefab has no resolvable nested prefab instances.");

                return recipe;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
        }

        private static bool HasNestedInstances(Transform parent)
        {
            foreach (Transform child in parent)
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(child.gameObject) || HasNestedInstances(child))
                    return true;
            }

            return false;
        }

        private static void ReadNode(Transform node, Matrix4x4 parentPose, string parentGroup,
            bool parentActive, Recipe recipe, List<string> warnings)
        {
            GameObject instance = node.gameObject;
            bool active = parentActive && instance.activeSelf;
            Matrix4x4 pose = parentPose * Matrix4x4.TRS(node.localPosition, node.localRotation, node.localScale);
            string objectPath = string.IsNullOrEmpty(parentGroup) ? node.name : $"{parentGroup} / {node.name}";

            if (PrefabUtility.IsAnyPrefabInstanceRoot(instance))
            {
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instance);
                GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (!source || assetPath == recipe.sourcePrefab)
                    throw new InvalidDataException($"Could not resolve the nested prefab at '{objectPath}'.");

                string group = string.IsNullOrEmpty(parentGroup) ? "Root" : parentGroup;
                recipe.entries.Add(MakeEntry(recipe.entries.Count + 1, group, node.name, source, assetPath, pose));
                if (source.activeSelf != active)
                    warnings.Add($"{objectPath}: the effective active state differs from the source prefab.");
                if (HasUnrepresentedOverrides(instance))
                    warnings.Add($"{objectPath}: nested prefab overrides other than placement or names will use source values.");
                return;
            }

            RecordWrapperWarning(node, objectPath, warnings);
            foreach (Transform child in node)
                ReadNode(child, pose, objectPath, active, recipe, warnings);
        }

        private static void RecordWrapperWarning(Transform node, string path, List<string> warnings)
        {
            if (node.GetComponents<Component>().Length > 1 || node is RectTransform)
                warnings.Add($"{(string.IsNullOrEmpty(path) ? node.name : path)}: components on a non-prefab group will be omitted.");
        }

        private static bool HasUnrepresentedOverrides(GameObject instance)
        {
            if (PrefabUtility.GetAddedGameObjects(instance).Count > 0 ||
                PrefabUtility.GetRemovedGameObjects(instance).Count > 0 ||
                PrefabUtility.GetAddedComponents(instance).Count > 0 ||
                PrefabUtility.GetRemovedComponents(instance).Count > 0)
                return true;

            var sourceTransforms = new HashSet<Transform>();
            for (Transform source = PrefabUtility.GetCorrespondingObjectFromSource(instance.transform);
                 source && sourceTransforms.Add(source);
                 source = PrefabUtility.GetCorrespondingObjectFromSource(source))
            {
            }

            PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(instance);
            if (modifications == null)
                return false;

            foreach (PropertyModification modification in modifications)
            {
                if (modification == null || PrefabUtility.IsDefaultOverride(modification))
                    continue;

                string path = modification.propertyPath;
                if (path == "m_Name" && modification.target is GameObject)
                    continue;
                if (modification.target is Transform transform && sourceTransforms.Contains(transform) &&
                    IsPlacementProperty(path))
                    continue;

                return true;
            }

            return false;
        }

        private static bool IsPlacementProperty(string path)
        {
            return path != null && (path.StartsWith("m_LocalPosition.", StringComparison.Ordinal) ||
                   path.StartsWith("m_LocalRotation.", StringComparison.Ordinal) ||
                   path.StartsWith("m_LocalScale.", StringComparison.Ordinal) ||
                   path.StartsWith("m_LocalEulerAnglesHint.", StringComparison.Ordinal) ||
                   path == "m_RootOrder" || path == "m_ConstrainProportionsScale");
        }

        private static Entry MakeEntry(int id, string group, string label, GameObject prefab,
            string assetPath, Matrix4x4 pose)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid))
                throw new InvalidDataException($"No asset GUID found for '{assetPath}'.");

            Vector3 x = pose.GetColumn(0);
            Vector3 y = pose.GetColumn(1);
            Vector3 z = pose.GetColumn(2);
            Vector3 position = pose.GetColumn(3);
            var scale = new Vector3(x.magnitude, y.magnitude, z.magnitude);
            if (!IsFinite(position) || !IsFinite(scale) || scale.x <= 0 || scale.y <= 0 || scale.z <= 0)
                throw new InvalidDataException($"'{group} / {label}' has an invalid or zero-scale transform.");

            x /= scale.x;
            y /= scale.y;
            z /= scale.z;
            if (Vector3.Dot(Vector3.Cross(x, y), z) < 0)
                throw new InvalidDataException($"'{group} / {label}' is mirrored. The assembly format requires positive scale.");
            if (Mathf.Abs(Vector3.Dot(x, y)) > OrthogonalityTolerance ||
                Mathf.Abs(Vector3.Dot(x, z)) > OrthogonalityTolerance ||
                Mathf.Abs(Vector3.Dot(y, z)) > OrthogonalityTolerance)
            {
                throw new InvalidDataException($"'{group} / {label}' has shear from rotated, non-uniformly scaled parents. " +
                                               "The assembly format stores only position, rotation and scale.");
            }

            return new Entry
            {
                id = id,
                group = group,
                name = label,
                prefab = prefab.name,
                guid = guid,
                assetPath = assetPath,
                localPosition = ToArray(position),
                localEulerAngles = ToArray(Quaternion.LookRotation(z, y).eulerAngles),
                localScale = ToArray(scale)
            };
        }

        private static bool IsFinite(Vector3 vector)
        {
            return IsFinite(vector.x) && IsFinite(vector.y) && IsFinite(vector.z);
        }

        private static bool IsFinite(float number)
        {
            return !float.IsNaN(number) && !float.IsInfinity(number);
        }

        private static float[] ToArray(Vector3 vector)
        {
            return new[] { vector.x, vector.y, vector.z };
        }
    }
}
