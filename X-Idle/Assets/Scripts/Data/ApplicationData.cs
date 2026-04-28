using System;
using System.Collections.Generic;
using System.Linq;
using Common.Lifecycle;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.Enum;
using Data.Events;
using Data.Factory;
using Data.Interface;
using Data.Loader;
using Data.Model;
using Data.Model.Error;
using Data.Model.Popup;
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
        private UserData m_userData;
        private IBuildingFactory m_buildingFactory;
        private readonly UserDataProcessor m_userDataProcessor = new();
        private readonly IDataLoader<UserData> m_userDataLoader = new UserDataLoader();

        public IList<BuildingModel> UserBuildings => m_userData.UserBuildingsData.BuildingsMap.Values.ToList();
        public UserResources UserResources => m_userData.UserResources;
        public UserBuildingsData UserBuildingsData => m_userData.UserBuildingsData;
        public event Action<BuildingModel> OnBuildingCreated;
        public event Action<ActionErrorModel> OnActionError;
        public event Action<BuildingModel> OnBuildingUpdated;

        public event Action<UserResources> OnUserResourcesChanged;


        public void InitApplicationData(Action complete)
        {
            if (m_assetLibrary == null)
                m_assetLibrary = Resources.Load<AssetLibrary>("AssetLibrary");
            m_userData = m_userDataLoader.Load();


            if (m_userData == null)
            {
                var sceneData = Resources.Load<SceneTemplate>("SceneTemplate");

                List<BuildingModel> buildingsModel = new List<BuildingModel>();

                foreach (var buildingContext in sceneData.SceneBuildings)
                {
                    m_assetLibrary.TryGetBuildingTemplate(buildingContext.BuildingTemplateId, out BuildingTemplate buildingTemplate);
                    buildingsModel.Add(new BuildingModel(buildingContext.Id, buildingTemplate, 1));
                }

                m_userData = new UserData(buildingsModel);

                m_userDataLoader.Save(m_userData);
            }


            m_buildingFactory = new BuildingFactory(m_assetLibrary);

            m_userData.UserResources.OnUserResourcesChanged += OnResourcesChangedHandler;
            m_userDataProcessor.StartProcessing(m_userData);

            complete?.Invoke();
        }


        private void OnResourcesChangedHandler(UserResources data)
        {
            OnUserResourcesChanged?.Invoke(data);
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
                    else
                    {
                        m_assetLibrary.TryGetBuildingTemplate(args.TemplateId, out BuildingTemplate buildingTemplate);

                        OnActionError?.Invoke(new ActionErrorModel(ActionErrorType.CreateBuilding,
                            DataUtility.GetCreateBuildingRequirementContext(m_userData.UserResources, m_userData.UserBuildingsData, buildingTemplate)));
                    }

                    break;
                case BuildingActionType.UpgradeBuildingRequest:

                    if (m_buildingFactory.TryUpgradeBuilding(m_userData, args, out BuildingModel updatedBuildingModel))
                    {
                        OnBuildingUpdated?.Invoke(updatedBuildingModel);
                    }

                    break;
            }
        }


        public UpgradeBuildingContext GetUpgradeBuildingContext(BuildingModel buildingModel) =>
            DataUtility.GetUpgradeBuildingContext(buildingModel, m_userData.UserResources, m_userData.UserBuildingsData);


        public void OnApplicationQuit()
        {
            m_userDataProcessor.StopProcessing();
        }

        public void OnApplicationFocus(bool hasFocus)
        {
        }

        public void OnApplicationPause(bool pauseStatus)
        {
        }
    }
}