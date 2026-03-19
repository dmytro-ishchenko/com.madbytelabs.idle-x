using Data;
using SceneLoader;

namespace GameLoop
{
    internal class Gameplay : IGameplay
    {
        public Gameplay(IApplicationData data, IAppSceneLoader sceneLoader)
        {
            m_appData = data;
        }

        private readonly IApplicationData m_appData;
    }
}