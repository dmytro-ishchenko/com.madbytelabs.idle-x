using System.Collections.Generic;
using System.IO;
using Common;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.GameResources;
using Data.ContentLibrary.Templates.Placeholder;
using Data.Enum;
using Data.Interface;
using Data.Persistent;
using NUnit.Framework;
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

        [MenuItem("AFTER/Library/CreateSaveData", false, 100)]
        public static void CreateSaveData()
        {
            var library = LoadAssetLibrary();
            var scenaData = AssetDatabase.LoadAssetAtPath(AssetPath.SCENE_TEMPLATE_PATH, typeof(SceneTemplate)) as SceneTemplate;

            List<PlaceHolderSaveModel> placeholders = new();
            List<BuildingSaveModel> buildings = new();

            foreach (var building in scenaData.SceneBuildings)
            {
                placeholders.Add(new PlaceHolderSaveModel(building.Id, building.BuildingTemplateId, building.PlaceHolderType, building.PlaceHolderStatus));
                buildings.Add(new BuildingSaveModel(building.Id, building.BuildingTemplateId, 1));
            }


            List<ResourceSaveModel> resources = new();

            resources.Add(new ResourceSaveModel(GameResourceType.Scrap, 0));
            resources.Add(new ResourceSaveModel(GameResourceType.Data, 0));
            resources.Add(new ResourceSaveModel(GameResourceType.Energy, 0));
            resources.Add(new ResourceSaveModel(GameResourceType.Food, 0));
            resources.Add(new ResourceSaveModel(GameResourceType.Parts, 0));
            resources.Add(new ResourceSaveModel(GameResourceType.Water, 0));

            var saveModel = new SaveModel(placeholders, buildings, resources);

            string saveFilePath = "Assets/AssetDataBase/Resources/SaveTemplate.asset";
            var scriptableObject = CreateInstance<SaveTemplate>();

            scriptableObject.InitSaveModel(saveModel);

            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);

            AssetDatabase.CreateAsset(scriptableObject, saveFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}