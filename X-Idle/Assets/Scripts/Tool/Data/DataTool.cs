using System.IO;
using Common;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;
using Data.ContentLibrary.Templates.Placeholder;
using Data.Interface;
using UnityEditor;
using UnityEngine;

namespace Tool.Data
{
    public class DataTool : Editor
    {
        static AssetLibrary LoadAssetLibrary()
        {
            return AssetDatabase.LoadAssetAtPath(AssetPath.LIBRARY_PATH, typeof(AssetLibrary)) as AssetLibrary;
        }


        [MenuItem("AFTER/Library/Create Library", false, 1)]
        public static void CreateNewLibrary()
        {
            var scriptableObject = CreateInstance<AssetLibrary>();

            AssetDatabase.CreateAsset(scriptableObject, "Assets/AssetDataBase/AssetLibrary.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("AFTER/Library/Create PlaceHolder Requirement", false, 1)]
        public static void CreatePlaceHolderRequirement()
        {
            
            var scriptableObject = CreateInstance<PlaceholderMapTemplate>();

            AssetDatabase.CreateAsset(scriptableObject, "Assets/AssetDataBase/PlaceholderRequirementsMap.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        [MenuItem("AFTER/Library/Create New Building", false, 2)]
        public static void CreateNewContent()
        {
            var library = LoadAssetLibrary();

            var content = CreateInstance<BuildingTemplate>();

            var newId = ContentUtility.GetId();

            content.SetId(newId);
            AssetDatabase.CreateAsset(content, Path.Combine(AssetPath.BUILDINGS_PATH, "NewBuilding" + ".asset"));
            library.AddContent(newId, content);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("AFTER/Library/Game Resource", false, 4)]
        public static void CreateGameResource()
        {
            var library = LoadAssetLibrary();

            var resource = CreateInstance<GameResourcesTemplate>();

            var newId = ContentUtility.GetId();

            resource.SetId(newId);
            AssetDatabase.CreateAsset(resource, Path.Combine(AssetPath.GAME_RESOURCES_PATH, "NewGameResource" + ".asset"));


            library.AddResource(resource);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        [MenuItem("AFTER/Library/Rescan Library", false, 100)]
        public static void RescanLibrary()
        {
            var library = LoadAssetLibrary();

            library.ClearLibrary();

            var buildingTemplates = GetAllInstances<BuildingTemplate>();
            var resourceTemplates = GetAllInstances<GameResourcesTemplate>();

            foreach (var template in buildingTemplates)
            {
                library.AddContent(template.Id, template);
            }

            foreach (var template in resourceTemplates)
            {
                library.AddResource(template);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static T[] GetAllInstances<T>() where T : ScriptableObject, ITemplate
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/AssetDataBase" });
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var instance = AssetDatabase.LoadAssetAtPath<T>(path);
                a[i] = instance;
            }

            return a;
        }

        public static T[] GetAllInstances<T>(string folder) where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder });
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                var instance = AssetDatabase.LoadAssetAtPath<T>(path);
                a[i] = instance;
            }

            return a;
        }
    }
}