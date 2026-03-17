using System.IO;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;
using UnityEditor;

namespace Tool.Data
{
    public class DataTool : Editor
    {
        static AssetLibrary LoadAssetLibrary()
        {
            return AssetDatabase.LoadAssetAtPath(LibraryUtility.LIBRARY_PATH, typeof(AssetLibrary)) as AssetLibrary;
        }


        [MenuItem("AFTER/Library/Create Library", false, 1)]
        public static void CreateNewLibrary()
        {
            var scriptableObject = CreateInstance<AssetLibrary>();

            AssetDatabase.CreateAsset(scriptableObject, "Assets/AssetDataBase/AssetLibrary.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        [MenuItem("AFTER/Library/Create New Content", false, 2)]
        public static void CreateNewContent()
        {
            var library = LoadAssetLibrary();

            var content = CreateInstance<ContentTemplate>();

            var newId = ContentUtility.GetId();

            content.SetId(ContentUtility.GetId());
            AssetDatabase.CreateAsset(content, Path.Combine(LibraryUtility.CONTENT_PATH, "NewContent" + ".asset"));
            library.AddContent(newId, content);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("AFTER/Library/Context/Building Context", false, 3)]
        public static void CreateBuildingContext()
        {
            var library = LoadAssetLibrary();

            var context = CreateInstance<BuildingContext>();

            var newId = ContentUtility.GetId();

            context.SetId(newId);
            AssetDatabase.CreateAsset(context, Path.Combine(LibraryUtility.CONTEXT_PATH, "NewBuildingContext" + ".asset"));

            library.AddContext(newId, context);

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
            AssetDatabase.CreateAsset(resource, Path.Combine(LibraryUtility.GAME_RESOURCES_PATH, "NewGameResource" + ".asset"));


            library.AddResource(newId, resource);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}