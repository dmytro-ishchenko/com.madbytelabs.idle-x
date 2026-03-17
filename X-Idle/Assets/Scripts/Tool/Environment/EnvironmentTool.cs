using Data.ContentLibrary;
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

        [MenuItem("AFTER/Scene/Load Content", false, 2)]
        public static void LoadContent()
        {
        }

        [MenuItem("AFTER/Scene/Unload Content", false, 3)]
        public static void UnloadContent()
        {
        }
    }
}