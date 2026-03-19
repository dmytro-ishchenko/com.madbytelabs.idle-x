using Common;
using Common.Enum;
using Common.Events;
using Common.Pattern.BobbleEvent;
using GameEnvironment;
using SceneLoader;
using UI.Controller;
using UnityEngine.SceneManagement;

namespace UI
{
    internal class GameUI : IGameUI, IRootEventHandler
    {
        public GameUI(IEnvironment environment, IAppSceneLoader sceneLoader)
        {
            m_environment = environment;
            m_sceneLoader = sceneLoader;

            m_environment.OnSelectContent += OnSelectContentHandler;
            m_sceneLoader.SceneNotify.OnSceneLoaded += OnSceneLoadedHandler;
        }

        private readonly IEnvironment m_environment;

        private readonly IAppSceneLoader m_sceneLoader;

        private IGameUIController m_gameUIController;

        private void OnSceneLoadedHandler(Scene scene)
        {
            if (scene.name.Equals(nameof(SceneName.Game)))
            {
                m_gameUIController = scene.GetComponent<IGameUIController>();
            }
        }

        private void OnSelectContentHandler(SelectContentEventArgs args)
        {
            m_gameUIController.ShowSelectContentPopup(args.ContentModel);
        }

        public void Handle(NodeEvent evt)
        {
            
        }
    }
}