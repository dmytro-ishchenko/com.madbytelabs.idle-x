using System.IO;
using Common;
using Sound;
using UnityEditor;

namespace Tool.Sound
{
    public class SoundTool : Editor
    {
        [MenuItem("AFTER/Sound/Sound Template", false, 5)]
        public static void CreateGameResource()
        {
            var template = CreateInstance<SoundTemplate>();

            if (!Directory.Exists(AssetPath.GAME_SOUND_PATH))
                Directory.CreateDirectory(AssetPath.GAME_SOUND_PATH);
            AssetDatabase.CreateAsset(template, Path.Combine(AssetPath.GAME_SOUND_PATH, "SoundTemplate" + ".asset"));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}