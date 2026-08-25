using System;
using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary;
using Data.Enum;
using Data.Model;
using Data.Model.Popup;
using Data.Utility;
using UnityEngine;

namespace Data.Processor
{
    internal class UserDataProcessor
    {
        private readonly float m_tickTime = 1.0f;
        private UserResources m_userResources;
        private UserBuildingsData m_userBuildingsData;
        private BuildingModel m_mainBuildingModel;
        private IAssetLibrary m_assetLibrary;
        private readonly IProcessor m_resourcesProcessor = new Processor();
        private Dictionary<GameResourceType, ProductionModel> m_productionMap = new();

        public void Init(IAssetLibrary assetLibrary, UserData userData)
        {
            m_assetLibrary = assetLibrary;
            m_userResources = userData.UserResources;
            m_userBuildingsData = userData.UserBuildingsData;
            m_userBuildingsData.TryGetBuildingsByType(BuildingType.MainBuilding, out var mainBuildingModels);
            m_mainBuildingModel = mainBuildingModels.ElementAt(0);
        }

        public void StartProcessing()
        {
            m_resourcesProcessor.OnProcess += OnProcessHandler;
            m_resourcesProcessor.StartProcess(m_tickTime);
        }

        private void OnProcessHandler()
        {
            CalculateProductionMap();
            CalculateProduction();
            m_userResources.InvokeUpdateResources();
        }

        void CalculateProductionMap()
        {
            m_productionMap.Clear();


            var productionResourcesTypes = m_userBuildingsData.GetProductionResources();

            foreach (var productionType in productionResourcesTypes)
            {
                CalculateProductionMap(productionType);
            }
        }

        void CalculateProductionMap(GameResourceType resourceType)
        {
            if (NeedToAddToProductionMap(resourceType))
            {
                if (m_userBuildingsData.TryGetBuildingsByResourceType(resourceType, out var production))
                {
                    float productionAmount = 0;

                    var resourceUse = production.ElementAt(0).Template.BuildingContext.ResourcesUse;
                    if (resourceUse is { Count: > 0 })
                    {
                        float productionMultiplier = 1;

                        foreach (var resourceUseModel in resourceUse)
                        {
                            if (!m_productionMap.ContainsKey(resourceUseModel.GameResource.GameResourceType))
                            {
                                CalculateProductionMap(resourceUseModel.GameResource.GameResourceType);
                            }

                            m_productionMap.TryGetValue(resourceUseModel.GameResource.GameResourceType, out var productionModel);

                            var currentAmount = m_userResources.GetGameResourceValue(resourceType) + productionAmount;

                            float coverage = Mathf.Clamp01(currentAmount / productionModel.UsedAmount);

                            if (coverage < productionMultiplier)
                                productionMultiplier = coverage;
                        }

                        if (production is { Count: > 0 })
                        {
                            productionAmount = GetProductionAmount(production);
                        }

                        productionAmount *= productionMultiplier;

                        m_productionMap.Add(resourceType, new ProductionModel(resourceType, productionAmount, GetUseAmount(resourceType)));
                    }
                    else
                    {
                        if (m_productionMap.ContainsKey(resourceType))
                            return;
                        if (production is { Count: > 0 })
                        {
                            productionAmount = GetProductionAmount(production);
                        }
                        m_productionMap.Add(resourceType, new ProductionModel(resourceType, productionAmount, GetUseAmount(resourceType)));
                    }
                }
            }
        }


        void CalculateProduction()
        {
            m_userBuildingsData.TryGetBuildingsByType(BuildingType.Warehouse, out var warehouses);

            foreach (var productionModel in m_productionMap.Values)
            {
                m_assetLibrary.TryGetGameResource(productionModel.GameResourceType, out var resource);
                m_userBuildingsData.TryGetBuildingsByResourceType(productionModel.GameResourceType, out var productionBuildings);
                float maxCapacity = DataUtility.GetResourceMaxCapacity(resource, productionBuildings.Count, warehouses);

                float setAmount = m_userResources.GetGameResourceValue(productionModel.GameResourceType) + productionModel.ProductionAmount - productionModel.UsedAmount;

                var amount = Math.Clamp(setAmount, 0, maxCapacity);
                m_userResources.SetGameResource(productionModel.GameResourceType, amount);
            }
        }


        float GetUseAmount(GameResourceType resourceType)
        {
            float useAmount = 0;

            m_userBuildingsData.TryGetBuildingsByUseResourceType(resourceType, out var buildings);
            if (buildings is { Count: > 0 })
            {
                foreach (var building in buildings)
                {
                    foreach (var resourcesUseModel in building.Template.BuildingContext.ResourcesUse)
                    {
                        if (resourcesUseModel.GameResource.GameResourceType == resourceType)
                        {
                            useAmount += DataUtility.GetResourceUse(resourcesUseModel.Amount, building.Level, building.Template.BuildingContext.BuildingProduction.LevelMultiplier);
                        }
                    }
                }
            }

            return useAmount;
        }

        bool NeedToAddToProductionMap(GameResourceType resource)
        {
            switch (resource)
            {
                case GameResourceType.Data:
                case GameResourceType.Energy:
                case GameResourceType.Food:
                case GameResourceType.Parts:
                case GameResourceType.Scrap:
                case GameResourceType.Water:
                    return true;
            }

            return false;
        }

        (List<ResourceRewardModel>, List<StorageReachLimitModel>) CalculateOfflineProduction(float time, IAssetLibrary assetLibrary)
        {
            List<ResourceRewardModel> rewards = new();
            List<StorageReachLimitModel> resourcesStorages = new();

            assetLibrary.TryGetGameResource(GameResourceType.Storage, out var storage);
            m_userBuildingsData.TryGetBuildingsByType(BuildingType.Warehouse, out var warehouses);

            foreach (var element in m_productionMap.Values)
            {
                m_assetLibrary.TryGetGameResource(element.GameResourceType, out var resource);
                m_userBuildingsData.TryGetBuildingsByResourceType(element.GameResourceType, out var productionBuildings);
                float maxCapacity = DataUtility.GetResourceMaxCapacity(resource, productionBuildings.Count, warehouses);

                float productionDelta = (element.ProductionAmount - element.UsedAmount) * time;
                float setAmount = m_userResources.GetGameResourceValue(element.GameResourceType) + productionDelta;


                if (setAmount >= maxCapacity)
                {
                    resourcesStorages.Add(new StorageReachLimitModel(storage.Icon, resource.Name));
                }

                var amount = Math.Clamp(setAmount, 0, maxCapacity);

                var addedAmount = amount - m_userResources.GetGameResourceValue(element.GameResourceType);
                if (addedAmount > 1)
                    rewards.Add(new ResourceRewardModel(resource.Icon, resource.Name, addedAmount));

                m_userResources.SetGameResource(element.GameResourceType, amount);
            }

            return (rewards, resourcesStorages);
        }

        public void StopProcessing()
        {
            m_resourcesProcessor.StopProcess();
            m_resourcesProcessor.OnProcess -= OnProcessHandler;
        }

        float GetProductionAmount(ICollection<BuildingModel> list)
        {
            float amount = 0;

            foreach (var model in list)
            {
                amount += DataUtility.GetProductionAmount(m_mainBuildingModel, model);
            }

            return amount;
        }

        public OfflineReward UpdateAccordingCurrentTime(IAssetLibrary assetLibrary, long prevTime)
        {
            CalculateProductionMap();

            DateTime now = DateTime.UtcNow;
            var currentTime = ((DateTimeOffset)now).ToUnixTimeSeconds();
            var deltaTime = currentTime - prevTime;

            var result = CalculateOfflineProduction(deltaTime / m_tickTime, assetLibrary);

            return new OfflineReward(deltaTime, result.Item1, result.Item2);
        }


        struct ProductionModel
        {
            public ProductionModel(GameResourceType gameResourceType, float productionAmount, float usedAmount)
            {
                GameResourceType = gameResourceType;
                ProductionAmount = productionAmount;
                UsedAmount = usedAmount;
            }

            public GameResourceType GameResourceType { get; }
            public float ProductionAmount { get; }
            public float UsedAmount { get; }
        }
    }
}