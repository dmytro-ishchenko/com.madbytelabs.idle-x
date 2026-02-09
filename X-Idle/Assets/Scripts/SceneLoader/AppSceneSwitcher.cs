using Common.Enum;

namespace SceneLoader
{
    internal class AppSceneSwitcher: IAppSceneSwitcher
    {
        private IAppSceneManager m_sceneManager = new AppSceneManager();
        
        public void SwitchToScene(SceneName sceneName)
        {
            
        }

        public void AddScene(SceneName sceneName)
        {
           
        }

        public void CleanGameScene()
        {
           
        }
    }
} 