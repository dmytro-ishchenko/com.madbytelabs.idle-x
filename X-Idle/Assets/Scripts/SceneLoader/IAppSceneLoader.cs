
using Common.Enum;

namespace SceneLoader
{
    public interface IAppSceneLoader
    {
        IAppSceneNotify SceneNotify { get; }
        void SwitchToScene(SceneName sceneName);
        void AddScene(SceneName sceneName);
        void UnloadScene(SceneName sceneName);
    }
}