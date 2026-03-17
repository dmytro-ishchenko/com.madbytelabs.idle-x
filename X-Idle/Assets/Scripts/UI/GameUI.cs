using GameEnvironment;
using GameLoop;

namespace UI
{
    internal class GameUI : IGameUI
    {
        public GameUI(IEnvironment environment, IGameplay gameplay)
        {
            m_environment = environment;
            m_gameplay = gameplay;
        }

        private readonly IEnvironment m_environment;
        private readonly IGameplay m_gameplay;
    }
}