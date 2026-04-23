using System;
using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.Enum;
using Data.Events;
using Data.Interface;
using Data.Loader;
using Data.Model;
using UnityEngine;

namespace Data
{
    internal class ApplicationData : IApplicationData
    {
        private IAssetLibrary m_assetLibrary;
        private UserData m_userData;
        private readonly IDataLoader<UserData> m_userDataLoader = new UserDataLoader();
        public IList<BuildingModel> UserBuildings => m_userData.UserBuildingsData.BuildingsMap.Values.ToList();

        public void InitApplicationData(Action complete)
        {
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

            complete?.Invoke();
        }


        public IAssetLibrary AssetLibrary
        {
            get
            {
                if (m_assetLibrary == null)
                    m_assetLibrary = Resources.Load<AssetLibrary>("AssetLibrary");
                return m_assetLibrary;
            }
        }


        public IList<IBuildingTemplate> GetAvailableBuilding()
        {
            var userBuildings = m_userData.UserBuildingsData.BuildingsMap.Values.ToList();

            IList<IBuildingTemplate> templates = new List<IBuildingTemplate>();

            foreach (var buildingTemplate in AssetLibrary.Buildings)
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
            switch (args.BuildingActionType)
            {
                case BuildingActionType.CreateBuildingRequest:

                    break;
                case BuildingActionType.UpgradeBuildingRequest:
                    if (m_userData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out var building))
                    {
                    }

                    break;
            }
        }
    }
}