using Common;
using Common.Enum;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Events;
using GameEnvironment;
using SceneLoader;
using UI.Controller;
using UI.Model;
using UnityEngine.SceneManagement;

namespace UI
{
    internal class UISystem : IUISystem, IRootEventHandler
    {
        public UISystem(IApplicationData data, IEnvironment environment, IAppSceneLoader sceneLoader)
        {
            m_applicationData = data;
            m_environment = environment;
            m_sceneLoader = sceneLoader;

            m_environment.OnCreateBuildingRequest += OnCreateBuildingRequestHandler;
            m_environment.OnUpgradeBuildingRequest += OnUpgradeBuildingRequestHandler;
            m_sceneLoader.SceneNotify.OnSceneLoaded += OnSceneLoadedHandler;
        }

        private readonly IApplicationData m_applicationData;
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

        private void OnCreateBuildingRequestHandler(BuildingEventArgs args)
        {
           
            m_gameUIController.ShowCreateBuildingPopup( new CreateBuildingContext(args.BuildingModel.Id,m_applicationData.GetAvailableBuilding()));
        }

        private void OnUpgradeBuildingRequestHandler(BuildingEventArgs args)
        {
            m_gameUIController.ShowSelectBuildingPopup(args.BuildingModel);
        }

        public void Handle(NodeEvent evt)
        {
        }
    }
}