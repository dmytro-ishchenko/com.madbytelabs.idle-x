using System;
using Common.Enum;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    internal class AppSceneManager : IAppSceneManager
    {
        public AppSceneManager()
        {
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
            SceneManager.sceneUnloaded += OnSceneUnloadedHandler;
        }

        public event Action<Scene> OnSceneLoaded;
        public event Action<Scene> OnSceneUnloaded;

        public void LoadScene(SceneName sceneName, LoadSceneMode mode)
        {
            SceneManager.LoadSceneAsync(sceneName.ToString(), mode);
        }

        private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            OnSceneLoaded?.Invoke(scene);
        }

        public void UnloadScene(SceneName sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName.ToString());
        }

        private void OnSceneUnloadedHandler(Scene scene)
        {
            OnSceneUnloaded?.Invoke(scene);
        }
    }
}