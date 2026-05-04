using Common;
using Common.Enum;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Enum;
using Data.Events;
using Data.Model;
using Data.Model.Error;
using Data.Model.Popup;
using GameEnvironment;
using SceneLoader;
using UI.Controller;
using UI.Event;
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
            Subscribe<SelectPlaceHolderEventArgs>(SelectPlaceHolderHandler);
            Subscribe<UnlockPlaceholderEventArgs>(UnlockPlaceholderHandler);
            Subscribe<ShowBuildingsInfoEventArgs>(ShowBuildingsInfoHandler);
            Subscribe<ShowUpgradeBuildingEventArgs>(ShowUpgradeBuildingHandler);
            Subscribe<ShowDemolishBuildingEventArgs>(ShowDemolishBuildingHandler);
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
                m_applicationData.OnPlaceHolderStatusChanged += OnPlaceHolderStatusChangedHandler;

                m_gameUIController.UpdateUserInfo(m_applicationData.UserResources);

                foreach (var data in m_applicationData.UserPlaceHolderData)
                {
                    data.Value.SetTransform(m_environment.GetPlaceholderTransform(data.Key));
                }

                m_gameUIController.InitPlaceHoldersView(m_applicationData.UserPlaceHolderData);
            }
        }

        private void OnPlaceHolderStatusChangedHandler(PlaceholderModel model)
        {
            m_gameUIController.UpdatePlaceHoldersView(model);
        }


        private void OnUserResourcesChangedHandler(UserResources data)
        {
            m_gameUIController.UpdateUserInfo(data);
        }


        private void OnBuildingActionRequestHandler(BuildingRequestEventArgs args)
        {
            m_applicationData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out BuildingModel buildingModel);
            m_gameUIController.ShowSelectBuildingPopup(new SelectBuildingContext(args.BuildingId, buildingModel));
        }

        private void ShowBuildingsInfoHandler(ShowBuildingsInfoEventArgs args)
        {
        }


        private void ShowUpgradeBuildingHandler(ShowUpgradeBuildingEventArgs args)
        {
            m_gameUIController.ShowUpgradeBuildingPopup(m_applicationData.GetUpgradeBuildingContext(args.BuildingModel));
        }

        private void ShowDemolishBuildingHandler(ShowDemolishBuildingEventArgs args)
        {
        }

        private void BuildingProcessHandler(BuildingProcessEventArgs args)
        {
            m_applicationData.BuildingProcess(args);
        }

        private void OnActionErrorHandler(ActionErrorModel args)
        {
            m_gameUIController.ShowCreateBuildingErrorPopup(args);
        }

        private void SelectPlaceHolderHandler(SelectPlaceHolderEventArgs args)
        {
            m_gameUIController.SelectPlaceHolder(args, m_applicationData);
        }

        private void UnlockPlaceholderHandler(UnlockPlaceholderEventArgs args)
        {
            m_applicationData.UnlockPlaceHolder(args);
        }
    }
}