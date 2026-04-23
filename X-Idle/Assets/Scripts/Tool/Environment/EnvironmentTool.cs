using System.IO;
using Common;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context;
using GameEnvironment.Controller;
using UnityEditor;
using UnityEngine;

namespace Tool.Environment
{
    public class EnvironmentTool : Editor
    {
        [MenuItem("AFTER/Scene/Add PlaceHolder", false, 1)]
        public static void AddPlaceHolder()
        {
            PlaceholderRoot root = FindAnyObjectByType<PlaceholderRoot>();

            if (root == null)
            {
                Debug.LogError("Can't find PlaceholderRoot component");
                return;
            }

            GameObject placeholder = new GameObject();
            var component = placeholder.AddComponent<PlaceHolder>();

            component.SetId(ContentUtility.GetId());

            placeholder.transform.SetParent(root.transform);
            placeholder.transform.localPosition = Vector3.zero;
            placeholder.transform.localRotation = Quaternion.identity;

            root.AddPlaceholder(component);
        }

        [MenuItem("AFTER/Scene/Save Scene", false, 1)]
        public static void SaveScene()
        {
            PlaceholderRoot root = FindAnyObjectByType<PlaceholderRoot>();

            if (root == null)
            {
                Debug.LogError("Can't find PlaceholderRoot component");
                return;
            }

            var sceneTemplate = CreateInstance<SceneTemplate>();

            foreach (var placeholder in root.PlaceHolders)
            {
                sceneTemplate.AddSceneBuilding(new SceneBuildingContext(placeholder.Id, placeholder.ContentId, placeholder.transform.position));
            }

            if (!Directory.Exists(AssetPath.SCENE_PATH))
                Directory.CreateDirectory(AssetPath.SCENE_PATH);

            AssetDatabase.CreateAsset(sceneTemplate, Path.Combine(AssetPath.SCENE_PATH, "Scene" + ".asset"));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}