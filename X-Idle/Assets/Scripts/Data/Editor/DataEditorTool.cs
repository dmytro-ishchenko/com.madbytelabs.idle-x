using System.IO;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;
using UnityEditor;

namespace Data.Editor
{
    public class DataEditorTool : UnityEditor.Editor
    {
        private static readonly string s_libraryPath = "Assets/AssetDataBase/AssetLibrary.asset";
        private static readonly string s_contentPath = "Assets/AssetDataBase/Content/";
        private static readonly string s_contextPath = "Assets/AssetDataBase/Context/";
        private static readonly string s_gameResourcePath = "Assets/AssetDataBase/Game Resources/";

        static AssetLibrary LoadAssetLibrary()
        {
            return AssetDatabase.LoadAssetAtPath(s_libraryPath, typeof(AssetLibrary)) as AssetLibrary;
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

            content.SetId(newId);
            AssetDatabase.CreateAsset(content, Path.Combine(s_contentPath, "NewContent" + ".asset"));
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
            AssetDatabase.CreateAsset(context, Path.Combine(s_contextPath, "NewBuildingContext" + ".asset"));

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
            AssetDatabase.CreateAsset(resource, Path.Combine(s_gameResourcePath, "NewGameResource" + ".asset"));


            library.AddResource(newId, resource);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}