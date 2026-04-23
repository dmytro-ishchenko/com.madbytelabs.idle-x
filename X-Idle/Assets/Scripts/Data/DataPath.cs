using System.IO;
using UnityEngine;

namespace Data
{
    internal static class DataPath
    {
        private static string s_corePath = Path.Combine(Application.dataPath, "ApplicationCache");

        public static string UserDataPath
        {
            get
            {
                string combinedPath = Path.Combine(s_corePath, "UserData");
                if (!Directory.Exists(combinedPath))
                    Directory.CreateDirectory(combinedPath);
                return combinedPath;
            }
        }
    }
}