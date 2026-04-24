using System.Collections.Generic;
using System.Linq;
using Data.Enum;
using Data.Model;
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

        private readonly IProcessor m_scrapProcessor;
        private readonly IProcessor m_energyProcessor;
        private readonly IProcessor m_foodProcessor;
        private readonly IProcessor m_waterProcessor;

        public void StartProcessing(UserData userData)
        {
            m_userResources = userData.UserResources;

            ReviewUserBuildings(userData.UserBuildingsData.BuildingsMap.Values.ToList());
        }

        public void StopProcessing()
        {
            m_energyProcessor.StopProcess();
        }

        public void CreateBuildingProcess(UserBuildingsData userData, BuildingModel buildingModel)
        {
            if (buildingModel.Template.BuildingContext.BuildingProduction == null || buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate == null)
                return;

            switch (buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.ResourcesType)
            {
                case ResourcesType.Scrap:
                    ReviewScrap(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Scrap));
                    break;
                case ResourcesType.Energy:
                    ReviewEnergy(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Energy));
                    break;
                case ResourcesType.Food:
                    ReviewFood(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Food));
                    break;
                case ResourcesType.Water:
                    ReviewWater(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Water));
                    break;
            }
        }

        public void UpdateBuildingProcess(UserBuildingsData userData, BuildingModel buildingModel)
        {
            if (buildingModel.Template.BuildingContext.BuildingProduction == null || buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate == null)
                return;

            switch (buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.ResourcesType)
            {
                case ResourcesType.Scrap:
                    ReviewScrap(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Scrap));
                    break;
                case ResourcesType.Energy:
                    ReviewEnergy(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Energy));
                    break;
                case ResourcesType.Food:
                    ReviewFood(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Food));
                    break;
                case ResourcesType.Water:
                    ReviewWater(GetBuildingsByResourcesType(userData.BuildingsMap.Values.ToList(), ResourcesType.Water));
                    break;
            }
        }

        void ReviewUserBuildings(List<BuildingModel> list)
        {
            ReviewScrap(GetBuildingsByResourcesType(list, ResourcesType.Scrap));
            ReviewEnergy(GetBuildingsByResourcesType(list, ResourcesType.Energy));
            ReviewFood(GetBuildingsByResourcesType(list, ResourcesType.Food));
            ReviewWater(GetBuildingsByResourcesType(list, ResourcesType.Water));
        }

        List<BuildingModel> GetBuildingsByResourcesType(List<BuildingModel> list, ResourcesType resourcesType)
        {
            return list.Where(b => b.Template.BuildingContext.BuildingProduction != null &&
                                   b.Template.BuildingContext.BuildingProduction.ResourcesTemplate != null &&
                                   b.Template.BuildingContext.BuildingProduction.ResourcesTemplate.ResourcesType == resourcesType).ToList();
        }

        void ReviewScrap(List<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;

            m_scrapProcessor.InitTaskContext(GetAmount(list));

            if (!m_scrapProcessor.IsStarted)
            {
                m_scrapProcessor.OnProcess += (value) => { m_userResources.SetScrap(m_userResources.Scrap + value); };
                m_scrapProcessor.StartProcess();
            }
        }

        void ReviewEnergy(List<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;


            m_energyProcessor.InitTaskContext(GetAmount(list));

            if (!m_energyProcessor.IsStarted)
            {
                m_energyProcessor.OnProcess += (value) => { m_userResources.SetEnergy(m_userResources.Energy + value); };
                m_energyProcessor.StartProcess();
            }
        }

        void ReviewFood(List<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;

            m_foodProcessor.InitTaskContext(GetAmount(list));

            if (!m_foodProcessor.IsStarted)
            {
                m_foodProcessor.OnProcess += (value) => { m_userResources.SetFood(m_userResources.Food + value); };
                m_foodProcessor.StartProcess();
            }
        }

        void ReviewWater(List<BuildingModel> list)
        {
            if (list == null || list.Count == 0)
                return;

            m_waterProcessor.InitTaskContext(GetAmount(list));

            if (!m_waterProcessor.IsStarted)
            {
                m_waterProcessor.OnProcess += (value) => { m_userResources.SetWater(m_userResources.Water + value); };
                m_waterProcessor.StartProcess();
            }
        }

        int GetAmount(List<BuildingModel> list)
        {
            int amount = 0;
            foreach (var model in list)
            {
                amount += model.Template.BuildingContext.BuildingProduction.AmountPerSecond;
            }

            return amount;
        }
    }
}