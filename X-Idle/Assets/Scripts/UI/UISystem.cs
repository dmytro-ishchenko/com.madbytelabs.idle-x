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
            m_applicationData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out BuildingModel buildingModel);
            switch (buildingModel.Template.BuildingContext.BuildingType)
            {
                case BuildingType.DestroyedBuilding:
                    m_gameUIController.ShowCreateBuildingPopup(new CreateBuildingContext(args.BuildingId, m_applicationData.GetAvailableBuilding()));
                    break;
                default:
                    m_gameUIController.ShowUpgradeBuildingPopup(new UpgradeBuildingContext(m_applicationData.UserResources, m_applicationData.UserBuildingsData, buildingModel));
                    break;
            }
        }

        private void BuildingProcessHandler(BuildingProcessEventArgs args)
        {
            m_applicationData.BuildingProcess(args);
        }

        private void OnActionErrorHandler(ActionErrorModel args)
        {
            m_gameUIController.ShowCreateBuildingErrorPopup(new CreateBuildingErrorContext(m_applicationData.UserResources, m_applicationData.UserBuildingsData, args.BuildingTemplate));
        }
    }
}