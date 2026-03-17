using System.Collections.Generic;
using System.IO;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;
using UnityEditor;

namespace Tool
{
    public class LibraryUtility : Editor
    {
        private static readonly string s_libraryPath = "Assets/AssetDataBase/AssetLibrary.asset";

        public static AssetLibrary LoadLibrary()
        {
            AssetLibrary library;

            if (File.Exists(s_libraryPath))
            {
                library = (AssetLibrary)AssetDatabase.LoadAssetAtPath(s_libraryPath, typeof(AssetLibrary));

                CheckNull(library.ContentMap);

                CheckNull(library.ContextMap);

                CheckNull(library.GameResourcesMap);
            }
            else
            {
                library = (AssetLibrary)CreateInstance(typeof(AssetLibrary));
                AssetDatabase.CreateAsset(library, s_libraryPath);
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
    }
}