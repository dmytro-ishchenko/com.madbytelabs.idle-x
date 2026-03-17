using Data;
using SceneLoader;

namespace GameLoop
{
    internal class Gameplay : IGameplay
    {
        public Gameplay(IApplicationData data, IAppSceneLoader sceneLoader)
        {
            m_appData = data;
            _mSceneLoader = sceneLoader;
        }

        private readonly IApplicationData m_appData;
        private readonly IAppSceneLoader _mSceneLoader;
    }
}