using Common;
using Common.Enum;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Events;
using Data.Model;
using Data.Model.Popup;
using GameEnvironment;
using SceneLoader;
using UI.Controller;
using UI.Enum;
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
            Subscribe<ShowDismantleBuildingEventArgs>(ShowDismantleBuildingHandler);
            Subscribe<ShowResetProgressEventArgs>(ShowResetProgressHandler);
            Subscribe<ResetProgressEventArgs>(ResetProgressEventHandler);
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
                m_applicationData.OnPlaceHolderStatusChanged += OnPlaceHolderStatusChangedHandler;

                m_applicationData.OnProgressReset += InitController;

                InitController();

                var offlineReward = m_applicationData.GetOfflineReward();
                if (offlineReward != null)
                    m_gameUIController.PopupManager.ShowPopup(PopupType.OfflineReward, offlineReward);
            }
        }

        void InitController()
        {
            m_gameUIController.UpdateUserInfo(m_applicationData.UserResources);

            foreach (var data in m_applicationData.UserPlaceHolderData)
            {
                data.Value.SetTransform(m_environment.GetPlaceholderTransform(data.Key));
            }

            m_gameUIController.InitPlaceHoldersView(m_applicationData.UserPlaceHolderData);
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
            m_gameUIController.PopupManager.ShowPopup(PopupType.SelectBuilding, new SelectBuildingContext(args.BuildingId, buildingModel));
        }

        private void ShowBuildingsInfoHandler(ShowBuildingsInfoEventArgs args)
        {
            m_gameUIController.PopupManager.ShowPopup(PopupType.BuildingInfo, m_applicationData.GetBuildingInfo(args.BuildingModel.Id));
        }


        private void ShowUpgradeBuildingHandler(ShowUpgradeBuildingEventArgs args)
        {
            m_gameUIController.PopupManager.ShowPopup(PopupType.UpgradeBuilding, m_applicationData.GetUpgradeBuildingContext(args.BuildingModel));
        }

        private void ShowDismantleBuildingHandler(ShowDismantleBuildingEventArgs args)
        {
            m_gameUIController.PopupManager.ShowPopup(PopupType.ConfirmDismantle, args);
        }

        private void BuildingProcessHandler(BuildingProcessEventArgs args)
        {
            m_applicationData.BuildingProcess(args);
        }

        private void SelectPlaceHolderHandler(SelectPlaceHolderEventArgs args)
        {
            m_gameUIController.PopupManager.SelectPlaceHolder(args, m_applicationData);
        }

        private void UnlockPlaceholderHandler(UnlockPlaceholderEventArgs args)
        {
            m_applicationData.UnlockPlaceHolder(args);
        }

        private void ShowResetProgressHandler(ShowResetProgressEventArgs args)
        {
            m_gameUIController.PopupManager.ShowPopup(PopupType.ConfirmResetProgress);
        }

        private void ResetProgressEventHandler(ResetProgressEventArgs args)
        {
            m_applicationData.ResetProgress();
        }
    }
}