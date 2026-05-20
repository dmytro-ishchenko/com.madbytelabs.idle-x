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
        private ICollection<BuildingModel> m_warehouseModels;
        private IAssetLibrary m_assetLibrary;
        private readonly IProcessor m_resourcesProcessor = new Processor();
        private Dictionary<GameResourceType, float> m_multiplierMap = new();
        private Dictionary<GameResourceType, float> m_useResourcesMap = new();
        private Dictionary<GameResourceType, float> m_productionResourcesMap = new();

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
            UseResourcesPerTime(m_tickTime);
            ProductionResourcesPerTime(m_tickTime);
            CompleteProduction();

            m_userResources.InvokeUpdateResources();
        }


        void UseResourcesPerTime(float seconds)
        {
            var usingResourcesList = m_userBuildingsData.GetUsingResources();
            m_userBuildingsData.TryGetBuildingsByType(BuildingType.Warehouse, out m_warehouseModels);

            if (usingResourcesList is { Count: > 0 })
            {
                foreach (var resource in usingResourcesList)
                {
                    float useAmount = 0;
                    float multiplier = 1;

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

                    float productionAmount = 0;

                    m_userBuildingsData.TryGetBuildingsByResourceType(resource, out var productionBuildings);

                    if (productionBuildings is { Count: > 0 })
                    {
                        productionAmount = GetProductionAmount(productionBuildings);
                    }

                    var currentAmount = m_userResources.GetGameResourceValue(resource) + productionAmount;

                    float coverage = Mathf.Clamp01(currentAmount / useAmount);

                    multiplier = Mathf.Min(multiplier, coverage);
                    m_multiplierMap.TryAdd(resource, multiplier);

                    m_useResourcesMap.Add(resource, useAmount * multiplier * seconds);
                }
            }
        }

        void ProductionResourcesPerTime(float seconds)
        {
            var productionResources = m_userBuildingsData.GetProductionResources();

            if (productionResources is { Count: > 0 })
            {
                foreach (var resource in productionResources)
                {
                    if (TryProcessResource(resource, out var value))
                    {
                        if (m_productionResourcesMap.ContainsKey(resource))
                        {
                            m_productionResourcesMap[resource] = m_productionResourcesMap[resource] + value * seconds;
                        }
                        else
                            m_productionResourcesMap.Add(resource, value * seconds);
                    }
                }
            }
        }

        void CompleteProduction()
        {
            foreach (var element in m_productionResourcesMap)
            {
                var production = element.Value;
                if (m_useResourcesMap.TryGetValue(element.Key, out var value))
                {
                    production -= value;
                    m_useResourcesMap.Remove(element.Key);
                }

                TryAddGameResource(element.Key, production, out var addedAmount);
            }

            m_productionResourcesMap.Clear();

            if (m_useResourcesMap.Count > 0)
            {
                foreach (var element in m_useResourcesMap)
                {
                    m_userResources.SetGameResource(element.Key, m_userResources.GetGameResourceValue(element.Key) - element.Value);
                }

                m_useResourcesMap.Clear();
            }

            m_multiplierMap.Clear();
        }

        List<ResourceRewardModel> CalculateOfflineProduction()
        {
            List<ResourceRewardModel> rewards = new();

            foreach (var element in m_productionResourcesMap)
            {
                var production = element.Value;
                if (m_useResourcesMap.TryGetValue(element.Key, out var value))
                {
                    production -= value;
                    m_useResourcesMap.Remove(element.Key);
                }

                m_assetLibrary.TryGetGameResource(element.Key, out var resource);
                bool addAllResources = TryAddGameResource(element.Key, production, out var addedAmount);
                if (addedAmount > 0)
                    rewards.Add(new ResourceRewardModel(resource.Icon, resource.Name, addedAmount, addAllResources));
            }

            m_productionResourcesMap.Clear();

            if (m_useResourcesMap.Count > 0)
            {
                foreach (var element in m_useResourcesMap)
                {
                    m_userResources.SetGameResource(element.Key, m_userResources.GetGameResourceValue(element.Key) - element.Value);
                }

                m_useResourcesMap.Clear();
            }

            m_multiplierMap.Clear();
            return rewards;
        }

        bool TryProcessResource(GameResourceType type, out float amount)
        {
            if (m_userBuildingsData.TryGetBuildingsByResourceType(type, out var buildings))
            {
                amount = GetProductionAmount(buildings);
                if (m_multiplierMap.TryGetValue(type, out var multiplier))
                    amount *= multiplier;
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

        bool TryAddGameResource(GameResourceType gameResourceType, float value, out float added)
        {
            m_assetLibrary.TryGetGameResource(gameResourceType, out var resource);

            float setAmount = m_userResources.GetGameResourceValue(gameResourceType) + value;
            float maxCapacity = DataUtility.GetResourceMaxCapacity(resource, m_warehouseModels);

            if (maxCapacity < setAmount)
            {
                var amount = Math.Clamp(m_userResources.GetGameResourceValue(gameResourceType) + value, 0, DataUtility.GetResourceMaxCapacity(resource, m_warehouseModels));
                m_userResources.SetGameResource(gameResourceType, amount);

                added = value - (setAmount - maxCapacity);
                return false;
            }

            m_userResources.SetGameResource(gameResourceType, m_userResources.GetGameResourceValue(gameResourceType) + value);
            added = value;
            return true;
        }

        float GetProductionAmount(ICollection<BuildingModel> list)
        {
            float amount = 0;

            foreach (var model in list)
            {
                amount += DataUtility.GetProductionAmount(m_mainBuildingModel, model, 1);
            }

            return amount;
        }

        public OfflineReward UpdateAccordingCurrentTime(long prevTime)
        {
            DateTime now = DateTime.UtcNow;
            var currentTime = ((DateTimeOffset)now).ToUnixTimeSeconds();
            var deltaTime = currentTime - prevTime;

            var time = deltaTime / m_tickTime;

            UseResourcesPerTime(time);
            ProductionResourcesPerTime(time);
            ProductionResourcesPerTime(time);

            return new OfflineReward(deltaTime, CalculateOfflineProduction());
        }
    }
}