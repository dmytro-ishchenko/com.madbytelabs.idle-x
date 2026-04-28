using System.Collections.Generic;
using System.Linq;
using Data.Enum;
using Data.Model;
using Data.Utility;
using UnityEngine;

namespace Data.Processor
{
    internal class UserDataProcessor
    {
        internal UserDataProcessor()
        {
            m_energyProcessor = new Processor();
            m_foodProcessor = new Processor();
            m_scrapProcessor = new Processor();
            m_waterProcessor = new Processor();
        }

        private UserResources m_userResources;
        private BuildingModel m_mainBuildingModel;

        private readonly IProcessor m_scrapProcessor;
        private readonly IProcessor m_energyProcessor;
        private readonly IProcessor m_foodProcessor;
        private readonly IProcessor m_waterProcessor;


        public void StartProcessing(UserData userData)
        {
            m_userResources = userData.UserResources;

            userData.UserBuildingsData.TryGetBuildingsByType(BuildingType.MainBuilding, out var mainBuildingModels);
            m_mainBuildingModel = mainBuildingModels.ElementAt(0);

            ReviewUserBuildings(userData.UserBuildingsData);
        }

        public void StopProcessing()
        {
            m_energyProcessor.StopProcess();
        }

        public void CreateBuildingProcess(UserBuildingsData buildingsData, BuildingModel buildingModel)
        {
            if (buildingModel.Template.BuildingContext.BuildingProduction == null || buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate == null)
                return;

            switch (buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.GameResourceType)
            {
                case GameResourceType.Scrap:
                    ReviewScrap(GetBuildingsByResourcesType(buildingsData, GameResourceType.Scrap));
                    break;
                case GameResourceType.Energy:
                    ReviewEnergy(GetBuildingsByResourcesType(buildingsData, GameResourceType.Energy));
                    break;
                case GameResourceType.Food:
                    ReviewFood(GetBuildingsByResourcesType(buildingsData, GameResourceType.Food));
                    break;
                case GameResourceType.Water:
                    ReviewWater(GetBuildingsByResourcesType(buildingsData, GameResourceType.Water));
                    break;
            }
        }

        public void UpdateBuildingProcess(UserBuildingsData buildingsData, BuildingModel buildingModel)
        {
            if (buildingModel.Template.BuildingContext.BuildingProduction == null || buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate == null)
                return;

            switch (buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.GameResourceType)
            {
                case GameResourceType.Scrap:
                    ReviewScrap(GetBuildingsByResourcesType(buildingsData, GameResourceType.Scrap));
                    break;
                case GameResourceType.Energy:
                    ReviewEnergy(GetBuildingsByResourcesType(buildingsData, GameResourceType.Energy));
                    break;
                case GameResourceType.Food:
                    ReviewFood(GetBuildingsByResourcesType(buildingsData, GameResourceType.Food));
                    break;
                case GameResourceType.Water:
                    ReviewWater(GetBuildingsByResourcesType(buildingsData, GameResourceType.Water));
                    break;
            }
        }

        void ReviewUserBuildings(UserBuildingsData buildingsData)
        {
            ReviewScrap(GetBuildingsByResourcesType(buildingsData, GameResourceType.Scrap));
            ReviewEnergy(GetBuildingsByResourcesType(buildingsData, GameResourceType.Energy));
            ReviewFood(GetBuildingsByResourcesType(buildingsData, GameResourceType.Food));
            ReviewWater(GetBuildingsByResourcesType(buildingsData, GameResourceType.Water));
        }

        ICollection<BuildingModel> GetBuildingsByResourcesType(UserBuildingsData buildingsData, GameResourceType gameResourceType)
        {
            if (buildingsData.TryGetBuildingsByResourcesType(gameResourceType, out var buildings))
            {
                return buildings;
            }

            return null;
        }

        void ReviewScrap(ICollection<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;

            m_scrapProcessor.InitTaskContext(GetAmount(list));

            if (!m_scrapProcessor.IsStarted)
            {
                m_scrapProcessor.OnProcess += (value) => { SetGameResource(GameResourceType.Scrap, value); };
                m_scrapProcessor.StartProcess();
            }
        }

        void ReviewEnergy(ICollection<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;


            m_energyProcessor.InitTaskContext(GetAmount(list));

            if (!m_energyProcessor.IsStarted)
            {
                m_energyProcessor.OnProcess += (value) => { SetGameResource(GameResourceType.Energy, value); };
                m_energyProcessor.StartProcess();
            }
        }

        void ReviewFood(ICollection<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;

            m_foodProcessor.InitTaskContext(GetAmount(list));

            if (!m_foodProcessor.IsStarted)
            {
                m_foodProcessor.OnProcess += (value) => { SetGameResource(GameResourceType.Food, value); };
                m_foodProcessor.StartProcess();
            }
        }

        void ReviewWater(ICollection<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;

            m_waterProcessor.InitTaskContext(GetAmount(list));

            if (!m_waterProcessor.IsStarted)
            {
                m_waterProcessor.OnProcess += (value) => { SetGameResource(GameResourceType.Water, value); };
                m_waterProcessor.StartProcess();
            }
        }

        void SetGameResource(GameResourceType gameResourceType, float value)
        {
            m_userResources.SetGameResource(gameResourceType, m_userResources.GetGameResourceValue(gameResourceType) + value);
        }

        float GetAmount(ICollection<BuildingModel> list)
        {
            float amount = 0;
            var energyFactor = 1;
            var waterFactor = 1;

            foreach (var model in list)
            {
                var value = (model.Template.BuildingContext.BuildingProduction.Amount + (model.Level - 1) * model.Template.BuildingContext.BuildingProduction.LevelMultiplier) *
                            energyFactor * waterFactor * DataUtility.GetMainBuildingBonus(m_mainBuildingModel.Level, m_mainBuildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier);
                amount += value;
            }

            return amount;
        }
    }
}