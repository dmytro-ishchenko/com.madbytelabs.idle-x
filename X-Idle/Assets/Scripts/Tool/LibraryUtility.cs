using System.Collections.Generic;
using System.IO;
using Common;
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
        public static AssetLibrary LoadLibrary()
        {
            AssetLibrary library;

            if (File.Exists(AssetPath.LIBRARY_PATH))
            {
                library = (AssetLibrary)AssetDatabase.LoadAssetAtPath(AssetPath.LIBRARY_PATH, typeof(AssetLibrary));

                // CheckNull(library.BuildingsMap);
                // CheckNull(library.BuildingsBuildingsContextMap);
                // CheckNull(library.GameResourcesMap);
            }
            else
            {
                library = (AssetLibrary)CreateInstance(typeof(AssetLibrary));
                AssetDatabase.CreateAsset(library, AssetPath.LIBRARY_PATH);
            }

            EditorUtility.SetDirty(library);

            return library;
        }

        static void CheckNull(Dictionary<string, BuildingTemplate> map)
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

        public static void ViewInProject(ScriptableObject obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);

            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
        }
    }
}