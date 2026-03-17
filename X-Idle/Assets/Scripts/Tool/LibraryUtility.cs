using System.Collections.Generic;
using System.IO;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;
using UnityEditor;
using UnityEngine;

namespace Tool
{
    public class LibraryUtility : Editor
    {
        public static readonly string LIBRARY_PATH = "Assets/AssetDataBase/AssetLibrary.asset";
        public static readonly string CONTENT_PATH= "Assets/AssetDataBase/Content/";
        public static readonly string CONTEXT_PATH = "Assets/AssetDataBase/Context/";
        public static readonly string GAME_RESOURCES_PATH = "Assets/AssetDataBase/Game Resources/";

        public static AssetLibrary LoadLibrary()
        {
            AssetLibrary library;

            if (File.Exists(LIBRARY_PATH))
            {
                library = (AssetLibrary)AssetDatabase.LoadAssetAtPath(LIBRARY_PATH, typeof(AssetLibrary));

                CheckNull(library.ContentMap);

                CheckNull(library.ContextMap);

                CheckNull(library.GameResourcesMap);
            }
            else
            {
                library = (AssetLibrary)CreateInstance(typeof(AssetLibrary));
                AssetDatabase.CreateAsset(library, LIBRARY_PATH);
            }

            EditorUtility.SetDirty(library);

            return library;
        }

        static void CheckNull(Dictionary<string, ContentTemplate> map)
        {
            foreach (var element in map)
            {
                if (element.Value == null)
                {
                    map.Remove(element.Key);
                    CheckNull(map);
                    break;
                }
            }
        }

        static void CheckNull(Dictionary<string, BuildingContext> map)
        {
            foreach (var element in map)
            {
                if (element.Value == null)
                {
                    map.Remove(element.Key);
                    CheckNull(map);
                    break;
                }
            }
        }

        static void CheckNull(Dictionary<string, GameResourcesTemplate> map)
        {
            foreach (var element in map)
            {
                if (element.Value == null)
                {
                    map.Remove(element.Key);
                    CheckNull(map);
                    break;
                }
            }
        }
        
        public static void ViewInProject(ScriptableObject obj) {
            var path = AssetDatabase.GetAssetPath(obj);

            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
        }
    }
}