
using Common.Enum;

namespace SceneLoader
{
    public interface IAppSceneSwitcher
    {
        void SwitchToScene(SceneName sceneName);
        void AddScene(SceneName sceneName);
        void CleanGameScene();
    }
}