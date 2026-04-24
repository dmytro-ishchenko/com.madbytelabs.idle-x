using System;
using System.Collections.Generic;
using System.Linq;
using Commom.Lifecycle;
using Common.Lifecycle;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.Enum;
using Data.Events;
using Data.Interface;
using Data.Loader;
using Data.Model;
using Data.Processor;
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
        private readonly UserDataProcessor m_userDataProcessor = new();
        private readonly IDataLoader<UserData> m_userDataLoader = new UserDataLoader();
        public IList<BuildingModel> UserBuildings => m_userData.UserBuildingsData.BuildingsMap.Values.ToList();
        public UserResources UserResources => m_userData.UserResources;
        public event Action<BuildingModel> OnBuildingCreated;
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

                var userBuilding = userBuildings.FirstOrDefault(b => b.Template.BuildingContext.BuildingType.Equals(buildingTemplate.BuildingContext.BuildingType));

                if (userBuilding == null || userBuilding.Template.BuildingContext.CanDuplicate)
                {
                    templates.Add(buildingTemplate);
                }
            }

            return templates;
        }

        public void BuildingProcess(BuildingProcessEventArgs args)
        {
            if (m_userData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out var building))
            {
                switch (args.BuildingActionType)
                {
                    case BuildingActionType.CreateBuildingRequest:
                        if (m_assetLibrary.TryGetBuildingTemplate(args.TemplateId, out var buildingTemplate))
                        {
                            building.SetTemplate(buildingTemplate);
                            OnBuildingCreated?.Invoke(building);

                            m_userDataProcessor.CreateBuildingProcess(m_userData.UserBuildingsData, building);
                        }

                        break;
                    case BuildingActionType.UpgradeBuildingRequest:


                        m_userDataProcessor.UpdateBuildingProcess(m_userData.UserBuildingsData, building);
                        break;
                }
            }
        }

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