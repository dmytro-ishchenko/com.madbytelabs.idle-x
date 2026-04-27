using Common;
using Common.Enum;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Enum;
using Data.Events;
using Data.Model;
using GameEnvironment;
using SceneLoader;
using UI.Controller;
using UI.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    internal class UISystem : BobbleDispatcher, IUISystem
    {
        public UISystem(IApplicationData data, IEnvironment environment, IAppSceneLoader sceneLoader)
        {
            m_applicationData = data;
            m_environment = environment;
            m_sceneLoader = sceneLoader;

            m_environment.OnBuildingActionRequest += OnBuildingActionRequestHandler;

            m_sceneLoader.SceneNotify.OnSceneLoaded += OnSceneLoadedHandler;
            Subscribe<BuildingProcessEventArgs>(BuildingProcessHandler);
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
                m_gameUIController.Node.SetDispatcher(this);
                m_applicationData.OnUserResourcesChanged += OnUserResourcesChangedHandler;
                m_applicationData.OnActionError += OnActionErrorHandler;

                m_gameUIController.UpdateUserInfo(m_applicationData.UserResources);
            }
        }


        private void OnUserResourcesChangedHandler(UserResources data)
        {
            m_gameUIController.UpdateUserInfo(data);
        }


        private void OnBuildingActionRequestHandler(BuildingRequestEventArgs args)
        {
            switch (args.BuildingActionType)
            {
                case BuildingActionType.CreateBuildingRequest:
                    m_gameUIController.ShowCreateBuildingPopup(new CreateBuildingContext(args.BuildingModel.Id, m_applicationData.GetAvailableBuilding()));
                    break;
                case BuildingActionType.UpgradeBuildingRequest:
                    m_gameUIController.ShowUpgradeBuildingPopup(new UpgradeBuildingContext(m_applicationData.UserResources, m_applicationData.UserBuildingsData, args.BuildingModel));
                    break;
            }
        }

        private void BuildingProcessHandler(BuildingProcessEventArgs args)
        {
            m_applicationData.BuildingProcess(args);
        }

        private void OnActionErrorHandler(ActionErrorModel args)
        {
            Debug.LogError($"ActionError {args.ActionErrorType}");
        }
    }
}