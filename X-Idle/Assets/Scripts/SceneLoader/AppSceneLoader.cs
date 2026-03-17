using Common.Enum;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    internal class AppSceneLoader : IAppSceneLoader
    {
        private readonly IAppSceneManager m_sceneManager = new AppSceneManager();

        public IAppSceneNotify SceneNotify => m_sceneManager;

        public void SwitchToScene(SceneName sceneName)
        {
            m_sceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        public void AddScene(SceneName sceneName)
        {
            m_sceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void CleanGameScene()
        {
        }
    }
}