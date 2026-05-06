using System;
using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary;
using Data.Enum;
using Data.Model;
using Data.Utility;

namespace Data.Processor
{
    internal class UserDataProcessor
    {
        private UserResources m_userResources;
        private UserBuildingsData m_userBuildingsData;
        private BuildingModel m_mainBuildingModel;
        private ICollection<BuildingModel> m_warehouseModels;
        private IAssetLibrary m_assetLibrary;
        private readonly IProcessor m_resourcesProcessor = new Processor();
        private readonly Dictionary<GameResourceType, float> m_productionMultipliers = new();

        public void StartProcessing(IAssetLibrary assetLibrary, UserData userData)
        {
            m_assetLibrary = assetLibrary;
            m_userResources = userData.UserResources;
            m_userBuildingsData = userData.UserBuildingsData;

            m_userBuildingsData.TryGetBuildingsByType(BuildingType.MainBuilding, out var mainBuildingModels);
            m_mainBuildingModel = mainBuildingModels.ElementAt(0);
            m_resourcesProcessor.OnProcess += OnProcessHandler;

            m_resourcesProcessor.StartProcess();
        }

        private void OnProcessHandler()
        {
            var usingResourcesList = m_userBuildingsData.GetUsingResources();
            m_productionMultipliers.Clear();

            m_userBuildingsData.TryGetBuildingsByType(BuildingType.Warehouse, out m_warehouseModels);

            if (usingResourcesList is { Count: > 0 })
            {
                foreach (var resource in usingResourcesList)
                {
                    float useAmount = 0;

                    m_userBuildingsData.TryGetBuildingsByUseResourceType(resource, out var buildings);

                    foreach (var building in buildings)
                    {
                        foreach (var resourcesUseModel in building.Template.BuildingContext.ResourcesUse)
                        {
                            if (resourcesUseModel.GameResource.GameResourceType == resource)
                            {
                                useAmount += DataUtility.GetResourceUse(resourcesUseModel.Amount, building.Level, building.Template.BuildingContext.BuildingProduction.LevelMultiplier);
                            }
                        }
                    }


                    var currentAmount = m_userResources.GetGameResourceValue(resource);

                    if (currentAmount < useAmount)
                    {
                        m_productionMultipliers.TryAdd(resource, currentAmount / useAmount);
                    }

                    m_userResources.SetGameResource(resource, currentAmount - useAmount);
                }
            }

            var productionResources = m_userBuildingsData.GetProductionResources();

            if (productionResources is { Count: > 0 })
            {
                foreach (var resource in productionResources)
                {
                    if (TryProcessResource(resource, out var value))
                    {
                        SetGameResource(resource, value);
                    }
                }
            }

            m_userResources.InvokeUpdateResources();
        }


        bool TryProcessResource(GameResourceType type, out float amount)
        {
            if (m_userBuildingsData.TryGetBuildingsByResourceType(type, out var buildings))
            {
                amount = GetAmount(buildings);
                return true;
            }

            amount = 0;
            return false;
        }

        public void StopProcessing()
        {
            m_resourcesProcessor.StopProcess();
            m_resourcesProcessor.OnProcess -= OnProcessHandler;
        }


        void SetGameResource(GameResourceType gameResourceType, float value)
        {
            m_assetLibrary.TryGetGameResource(gameResourceType, out var resource);

            var amount = Math.Clamp(m_userResources.GetGameResourceValue(gameResourceType) + value, 0, DataUtility.GetResourceMaxCapacity(resource, m_warehouseModels));

            m_userResources.SetGameResource(gameResourceType, amount);
        }

        float GetAmount(ICollection<BuildingModel> list)
        {
            float amount = 0;

            foreach (var model in list)
            {
                float useResourcesMultiplier = 1;

                if (model.Template.BuildingContext.ResourcesUse is { Count: > 0 })
                {
                    foreach (var resourcesUseModel in model.Template.BuildingContext.ResourcesUse)
                    {
                        if (m_productionMultipliers.TryGetValue(resourcesUseModel.GameResource.GameResourceType, out var multiplier))
                            useResourcesMultiplier *= multiplier;
                    }
                }

                var value = DataUtility.GetProductionAmount(m_mainBuildingModel, model, useResourcesMultiplier);


                amount += value;
            }

            return amount;
        }
    }
}