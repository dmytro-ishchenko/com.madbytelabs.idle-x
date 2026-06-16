using System;
using System.Collections.Generic;
using System.Linq;
using Common.Lifecycle;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Placeholder;
using Data.Enum;
using Data.Events;
using Data.Factory;
using Data.Interface;
using Data.Loader;
using Data.Model;
using Data.Model.Popup;
using Data.Persistent;
using Data.Processor;
using Data.Utility;
using UnityEngine;

namespace Data
{
    internal class ApplicationData : IApplicationData, ILifecycleDelegate
    {
        public ApplicationData()
        {
            LifecycleManager.Instance.AddDelegate(this);
        }

        private IAssetLibrary m_assetLibrary;
        private IPlaceholderMapTemplate m_placeholderMapTemplate;
        private SceneTemplate m_sceneTemplate;
        private UserData m_userData;
        private IBuildingFactory m_buildingFactory;
        private readonly UserDataProcessor m_userDataProcessor = new();
        private readonly IDataLoader<SaveModel> m_userDataLoader = new UserDataLoader();
        private OfflineReward m_offlineReward;
        private AppStatus m_currentStatus;
        public IList<BuildingModel> UserBuildings => m_userData.UserBuildingsData.BuildingsMap.Values.ToList();
        public UserResources UserResources => m_userData.UserResources;
        public UserBuildingsData UserBuildingsData => m_userData.UserBuildingsData;
        public IReadOnlyDictionary<string, PlaceholderModel> UserPlaceHolderData => m_userData.UserPlaceHolderData.PlaceHolderDataMap;
        public event Action<BuildingModel> OnBuildingCreated;
        public event Action<BuildingModel> OnBuildingDeleted;
        public event Action OnProgressReset;
        public event Action<PlaceholderModel> OnPlaceHolderStatusChanged;
        public event Action<AppStatus> OnAppStatusChanged;
        public event Action<UserResources> OnUserResourcesChanged;

        public void InitApplicationData(Action complete)
        {
            m_currentStatus = AppStatus.Initializing;
            OnAppStatusChanged?.Invoke(m_currentStatus);


            if (m_assetLibrary == null)
                m_assetLibrary = Resources.Load<AssetLibrary>("AssetLibrary");

            if (m_placeholderMapTemplate == null)
                m_placeholderMapTemplate = Resources.Load<PlaceholderMapTemplate>("PlaceholderMap");

            LoadSaveData();

            if (m_userData == null)
            {
                CreateDefaultSave();
            }

            m_buildingFactory = new BuildingFactory(m_assetLibrary);

            m_userData.UserResources.OnUserResourcesChanged += OnResourcesChangedHandler;
            m_userData.UserPlaceHolderData.OnPlaceHolderStatusChanged += OnPlaceHolderStatusChangedHandler;

            m_currentStatus = AppStatus.Init;
            OnAppStatusChanged?.Invoke(m_currentStatus);

            m_userDataProcessor.StartProcessing();

            m_currentStatus = AppStatus.Running;
            OnAppStatusChanged?.Invoke(m_currentStatus);
            complete?.Invoke();
        }

        void LoadSaveData()
        {
            var save = m_userDataLoader.Load();

            if (save != null)
            {
                m_userData = new UserData(m_assetLibrary, save);

                m_userDataProcessor.Init(m_assetLibrary, m_userData);
                m_offlineReward = m_userDataProcessor.UpdateAccordingCurrentTime(m_assetLibrary, save.Time);
            }
        }

        void CreateDefaultSave()
        {
            m_sceneTemplate = Resources.Load<SceneTemplate>("SceneTemplate");

            List<BuildingModel> buildingsModels = new List<BuildingModel>();

            UserPlaceHolderData placeHolderData = new();

            foreach (var buildingContext in m_sceneTemplate.SceneBuildings)
            {
                m_assetLibrary.TryGetBuildingTemplate(buildingContext.BuildingTemplateId, out BuildingTemplate buildingTemplate);
                buildingsModels.Add(new BuildingModel(buildingContext.Id, buildingTemplate, 1));

                placeHolderData.AddPlaceHolder(buildingContext.Id,
                    new PlaceholderModel(buildingContext.Id, buildingContext.BuildingTemplateId, buildingContext.PlaceHolderStatus, buildingContext.PlaceHolderType));
            }

            m_userData = new UserData(placeHolderData, buildingsModels);
            m_userDataProcessor.Init(m_assetLibrary, m_userData);

            SaveUserData();
        }

        private void OnResourcesChangedHandler(UserResources data)
        {
            OnUserResourcesChanged?.Invoke(data);
        }

        private void OnPlaceHolderStatusChangedHandler(PlaceholderModel model)
        {
            OnPlaceHolderStatusChanged?.Invoke(model);
        }

        public IList<IBuildingTemplate> GetAvailableBuilding()
        {
            var userBuildings = m_userData.UserBuildingsData.BuildingsMap.Values.ToList();

            IList<IBuildingTemplate> templates = new List<IBuildingTemplate>();

            foreach (var buildingTemplate in m_assetLibrary.Buildings)
            {
                if (buildingTemplate.BuildingContext.BuildingType == BuildingType.DestroyedBuilding || buildingTemplate.BuildingContext.BuildingType == BuildingType.MainBuilding)
                    continue;

                var buildings = userBuildings.Where(b => b.Template.BuildingContext.BuildingType.Equals(buildingTemplate.BuildingContext.BuildingType)).ToList();

                if (buildings.Count > 0)
                {
                    var building = buildings[0];
                    if (building.Template.BuildingContext.DuplicateModel.CanDuplicate && buildings.Count < building.Template.BuildingContext.DuplicateModel.MaxDuplicateAmount)
                        templates.Add(buildingTemplate);
                }
                else
                {
                    templates.Add(buildingTemplate);
                }
            }

            return templates;
        }

        public void BuildingProcess(BuildingProcessEventArgs args)
        {
            switch (args.BuildingActionType)
            {
                case BuildingActionType.CreateBuildingRequest:
                    if (m_buildingFactory.TryCreateBuilding(m_userData, args, out BuildingModel newBuildingModel))
                    {
                        OnBuildingCreated?.Invoke(newBuildingModel);
                    }

                    break;
                case BuildingActionType.UpgradeBuildingRequest:

                    if (m_buildingFactory.TryUpgradeBuilding(m_userData, args, out BuildingModel updatedBuildingModel))
                    {
                    }

                    break;
                case BuildingActionType.DismantleBuildingRequest:
                    if (m_buildingFactory.TryDismantleBuilding(m_assetLibrary, m_userData, args, out BuildingModel defaultBuildingModel))
                    {
                        OnBuildingDeleted?.Invoke(defaultBuildingModel);
                    }

                    break;
            }
        }


        public UpgradeBuildingContext GetUpgradeBuildingContext(BuildingModel buildingModel) =>
            DataUtility.GetUpgradeBuildingContext(buildingModel, m_assetLibrary, m_userData.UserResources, m_userData.UserBuildingsData);

        public PlaceHolderRequirementsModel GetPlaceHolderRequirements(string id)
        {
            return DataUtility.GetPlaceHolderRequirements(id, m_assetLibrary, m_placeholderMapTemplate, m_userData.UserPlaceHolderData, m_userData.UserBuildingsData, m_userData.UserResources);
        }

        public void UnlockPlaceHolder(UnlockPlaceholderEventArgs args)
        {
            DataUtility.UnlockPlaceHolder(args.PlaceHolderId, m_placeholderMapTemplate, m_userData.UserPlaceHolderData, m_userData.UserResources);
        }

        public SelectBuildingInfo GetBuildingInfo(string id)
        {
            return BuildingInfoBuilder.GetBuildingInfo(id, m_assetLibrary, m_userData.UserBuildingsData);
        }

        public CreateBuildingRequirementsContext GetCreateBuildingContext(IBuildingTemplate template)
        {
            return DataUtility.GetCreateBuildingRequirementContext(m_userData.UserResources, m_userData.UserBuildingsData, template);
        }

        public OfflineReward GetOfflineReward()
        {
            return m_offlineReward;
        }

        public void ResetProgress()
        {
            m_userDataProcessor.StopProcessing();

            m_userData.UserResources.OnUserResourcesChanged -= OnResourcesChangedHandler;
            m_userData.UserPlaceHolderData.OnPlaceHolderStatusChanged -= OnPlaceHolderStatusChangedHandler;

            CreateDefaultSave();

            m_userData.UserResources.OnUserResourcesChanged += OnResourcesChangedHandler;
            m_userData.UserPlaceHolderData.OnPlaceHolderStatusChanged += OnPlaceHolderStatusChangedHandler;

            m_userDataProcessor.StartProcessing();
            OnProgressReset?.Invoke();
        }

        public AppStatus AppStatus { get; }

        public ResourceInfoContext GetResourceInfoContext(GameResourceType resourceType)
        {
            m_assetLibrary.TryGetGameResource(resourceType, out var gameResource);
            m_assetLibrary.TryGetBuildingTemplateByResourceType(resourceType, out var buildingTemplate);
            UserBuildingsData.TryGetBuildingsByType(BuildingType.Warehouse, out var warehouses);

            float currentProduction = 0;
            float currentUse = 0;

            UserBuildingsData.TryGetBuildingsByResourceType(gameResource.GameResourceType, out var productionBuildings);

            float capacity = DataUtility.GetResourceMaxCapacity(gameResource, productionBuildings.Count, warehouses);

            ResourceInfoContext context = new ResourceInfoContext(gameResource.Name, gameResource.Icon, gameResource.Description, buildingTemplate.Name, buildingTemplate.Icon,
                currentProduction, UserResources.GetGameResourceValue(resourceType), capacity, currentUse);

            return context;
        }

        void SaveUserData()
        {
            m_userDataLoader.Save(m_userData.ToSave());
        }

        public void OnApplicationQuit()
        {
            m_userDataProcessor.StopProcessing();
            SaveUserData();
        }

        public void OnApplicationFocus(bool hasFocus)
        {
#if !UNITY_EDITOR
            if (!hasFocus)
                PauseGame();
            else
                ResumeGame();
#endif
        }

        public void OnApplicationPause(bool pauseStatus)
        {
#if !UNITY_EDITOR
            if (pauseStatus)
                PauseGame();
            else
                ResumeGame();
#endif
        }

        void PauseGame()
        {
            if (m_currentStatus != AppStatus.Pause)
            {
                m_userData.UserResources.OnUserResourcesChanged -= OnResourcesChangedHandler;
                m_userData.UserPlaceHolderData.OnPlaceHolderStatusChanged -= OnPlaceHolderStatusChangedHandler;
                m_userDataProcessor.StopProcessing();
                SaveUserData();
                m_currentStatus = AppStatus.Pause;
                OnAppStatusChanged?.Invoke(m_currentStatus);
            }
        }

        void ResumeGame()
        {
            if (m_currentStatus == AppStatus.Pause)
            {
                LoadSaveData();

                m_userData.UserResources.OnUserResourcesChanged += OnResourcesChangedHandler;
                m_userData.UserPlaceHolderData.OnPlaceHolderStatusChanged += OnPlaceHolderStatusChangedHandler;

                m_userDataProcessor.StartProcessing();

                m_currentStatus = AppStatus.Resuming;
                OnAppStatusChanged?.Invoke(m_currentStatus);

                m_currentStatus = AppStatus.Running;
                OnAppStatusChanged?.Invoke(m_currentStatus);
            }
        }
    }
}