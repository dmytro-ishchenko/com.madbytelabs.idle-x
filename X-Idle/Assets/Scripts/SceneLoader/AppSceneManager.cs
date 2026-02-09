using System;
using Common.Enum;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    internal class AppSceneManager : IAppSceneManager
    {
        public event Action<Scene> OnSceneLoaded;
        public event Action<Scene> OnSceneUnloaded;

        public void LoadScene(SceneName sceneName)
        {
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
            SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
        }

        private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedHandler;
            OnSceneLoaded?.Invoke(scene);
        }

        public void UnloadScene(SceneName sceneName)
        {
            SceneManager.sceneUnloaded += OnSceneUnloadedHandler;
            SceneManager.UnloadSceneAsync(sceneName.ToString());
        }

        private void OnSceneUnloadedHandler(Scene scene)
        {
            SceneManager.sceneUnloaded -= OnSceneUnloadedHandler;
            OnSceneUnloaded?.Invoke(scene);
        }
    }
}