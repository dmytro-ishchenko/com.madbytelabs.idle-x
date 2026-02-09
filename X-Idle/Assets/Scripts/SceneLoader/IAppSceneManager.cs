using System;
using Common.Enum;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    internal interface IAppSceneManager
    {
        void LoadScene(SceneName sceneName);
        void UnloadScene(SceneName sceneName);
        event Action<Scene> OnSceneLoaded;
        event Action<Scene> OnSceneUnloaded;
    }
}