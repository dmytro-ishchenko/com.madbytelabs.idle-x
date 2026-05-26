using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.GameResources;
using Data.ContentLibrary.Templates.Placeholder;
using Data.Enum;
using Data.Interface;
using Data.Model;
using Data.Model.Popup;
using UnityEngine;


namespace Data.Utility
{
    public static class DataUtility
    {
        internal static bool CanCreateBuilding(UserBuildingsData buildingsData, IBuildingTemplate template)
        {
            var buildRequirements = template.BuildingContext.CreateBuildingRequirements;

            if (buildRequirements is { Count: > 0 })
            {
                foreach (var element in buildRequirements)
                {
                    if (!buildingsData.TryGetBuildingsByType(element.BuildingType, out var requiredBuildings))
                    {
                        return false;
                    }

                    foreach (var model in requiredBuildings)
                    {
                        if (model.Level >= element.RequiredLevel)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        internal static CreateBuildingRequirementsContext GetCreateBuildingRequirementContext(UserResources userResources, UserBuildingsData userBuildingsData, IBuildingTemplate buildingTemplate)
        {
            List<BuildingContextModel> buildings = null;
            List<ResourceContextModel> resources = null;


            if (buildingTemplate.BuildingContext.CreateBuildingRequirements is { Count: > 0 })
            {
                buildings = new List<BuildingContextModel>();

                foreach (var element in buildingTemplate.BuildingContext.CreateBuildingRequirements)
                {
                    if (userBuildingsData.TryGetBuildingsByType(element.BuildingType, out var requiredBuildings))
                    {
                        int level = 0;
                        foreach (var model in requiredBuildings)
                        {
                            if (model.Level >= level)
                                level = model.Level;
                        }

                        var source = requiredBuildings.ElementAt(0);

                        buildings.Add(new BuildingContextModel(source.Template.BuildingContext.BuildingType, source.Template.Name, source.Level, element.RequiredLevel));
                    }
                }
            }

            if (buildingTemplate.BuildingContext.CreateCost.CostModels is { Count: > 0 })
            {
                resources = new List<ResourceContextModel>();
                foreach (var element in buildingTemplate.BuildingContext.CreateCost.CostModels)
                {
                    resources.Add(new ResourceContextModel(element.GameResource.GameResourceType, element.GameResource.Name, userResources.GetGameResourceValue(element.GameResource.GameResourceType),
                        element.Cost));
                }
            }

            return new CreateBuildingRequirementsContext(buildingTemplate.Name, buildingTemplate.Description, buildingTemplate.Icon, buildings, resources);
        }

        internal static UpgradeBuildingContext GetUpgradeBuildingContext(BuildingModel buildingModel, IAssetLibrary assetLibrary, UserResources userResources, UserBuildingsData userBuildingsData)
        {
            List<BuildingContextModel> buildings = null;
            List<ResourceContextModel> resources = null;
            IList<InfoElementModel> currentLevelInfoElements = null;
            IList<InfoElementModel> nextLevelInfoElements = null;

            int notMeetRequirements = 0;

            currentLevelInfoElements = BuildingInfoBuilder.GetBuildingInfo(buildingModel, assetLibrary, userBuildingsData, buildingModel.Level).effects;
            nextLevelInfoElements = BuildingInfoBuilder.GetBuildingInfo(buildingModel, assetLibrary, userBuildingsData, buildingModel.Level + 1).effects;

            if (buildingModel.Template.BuildingContext.UpgradeBuildingRequirements != null)
            {
                buildings = new List<BuildingContextModel>();
                foreach (var element in buildingModel.Template.BuildingContext.UpgradeBuildingRequirements)
                {
                    if (userBuildingsData.TryGetBuildingsByType(element.BuildingType, out var requiredBuildings))
                    {
                        int level = 0;

                        foreach (var requiredElement in requiredBuildings)
                        {
                            if (requiredElement.Level > level)
                            {
                                level = requiredElement.Level;
                            }
                        }

                        var building = requiredBuildings.ElementAt(0);

                        int requireLevel = Mathf.CeilToInt((float)(buildingModel.Level + 1) / element.LevelMultiplier);

                        buildings.Add(new BuildingContextModel(building.Template.BuildingContext.BuildingType, building.Template.Name, level, requireLevel));

                        if (requireLevel > level)
                        {
                            notMeetRequirements += 1;
                        }
                    }
                    else
                    {
                        assetLibrary.TryGetBuildingTemplateByType(element.BuildingType, out var buildingTemplate);

                        int requireLevel = Mathf.CeilToInt((float)(buildingModel.Level + 1) / element.LevelMultiplier);
                        buildings.Add(new BuildingContextModel(element.BuildingType, buildingTemplate.Name, 0, requireLevel));
                    }
                }
            }

            if (buildingModel.Template.BuildingContext.UpgradeCost.CostModels != null)
            {
                resources = new List<ResourceContextModel>();
                foreach (var element in buildingModel.Template.BuildingContext.UpgradeCost.CostModels)
                {
                    float requiredResource = UpgradeResourceCost(element.Cost, buildingModel.Level, element.CostGrowth);

                    if (userResources.GetGameResourceValue(element.GameResource.GameResourceType) < requiredResource)
                    {
                        notMeetRequirements += 1;
                    }

                    resources.Add(new ResourceContextModel(element.GameResource.GameResourceType, element.GameResource.Name, userResources.GetGameResourceValue(element.GameResource.GameResourceType),
                        requiredResource));
                }
            }

            OutputInfoModel outputModel = BuildingInfoBuilder.GetProductionInfoModel(buildingModel, userBuildingsData);

            IList<UseResourcesModel> useResourcesModels = BuildingInfoBuilder.GetUseResourcesModels(buildingModel, assetLibrary, userBuildingsData);

            return
                new UpgradeBuildingContext(buildingModel.Template.Name, buildingModel.Template.Description, buildingModel.Level, buildingModel.Template.Icon,
                    buildingModel.Id, buildingModel.Template.Id, outputModel, useResourcesModels, new RequirementsModel(buildings, resources), notMeetRequirements == 0);
        }

        internal static float UpgradeResourceCost(float cost, int buildingLevel, float costGrowth)
        {
            return cost * costGrowth * buildingLevel;
        }

        static float GetMainBuildingBonus(int level, float levelMultiplier)
        {
            return 1 + levelMultiplier * (level - 1);
        }

        internal static float GetResourceUse(float baseUse, int buildingLevel, float levelMultiplier)
        {
            return baseUse * levelMultiplier * buildingLevel;
        }

        internal static float GetProductionAmount(BuildingModel mainBuildingModel, BuildingModel buildingModel, float useResourcesMultiplier)
        {
            return (buildingModel.Template.BuildingContext.BuildingProduction.Amount * Mathf.Pow(buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier, buildingModel.Level - 1) +
                    (buildingModel.Level - 1)) *
                   GetMainBuildingBonus(mainBuildingModel.Level, mainBuildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier) * useResourcesMultiplier;
        }


        internal static float GetResourceMaxCapacity(IGameResourcesTemplate resource, ICollection<BuildingModel> warehouseModels)
        {
            if (warehouseModels is { Count: > 0 })
            {
                float maxCapacity = 0;
                foreach (var warehouseModel in warehouseModels)
                {
                    maxCapacity += GetCapacity(resource, warehouseModel.Level);
                }

                return maxCapacity;
            }
            else
                return resource.BaseCapacity;
        }

        internal static int GetCapacity(IGameResourcesTemplate resource, int warehouseLevel)
        {
            return Mathf.RoundToInt(resource.BaseCapacity + GetWarehouseBonus(warehouseLevel) * resource.StorageFactor);
        }

        internal static int GetWarehouseBonus(int level)
        {
            return level switch
            {
                0 => 0,
                1 => 100,
                2 => 250,
                3 => 500,
                4 => 900,
                5 => 1500,
                6 => 2400,
                7 => 3600,
                8 => 5200,
                9 => 7300,
                10 => 10000,
                _ => 10000 + (level - 10) * 3000
            };
        }

        internal static PlaceHolderRequirementsModel GetPlaceHolderRequirements(string placeHolderId,
            IAssetLibrary assetLibrary,
            IPlaceholderMapTemplate placeholderMapTemplate,
            UserPlaceHolderData userPlaceHolderData,
            UserBuildingsData userBuildingsData,
            UserResources userResources)
        {
            var placeHolder = userPlaceHolderData.PlaceHolderDataMap[placeHolderId];

            List<BuildingContextModel> buildingsContext = null;
            List<ResourceContextModel> resourcesContext = null;

            int notMeetRequirements = 0;

            if (placeholderMapTemplate.TryGetRequirementsByType(placeHolder.PlaceHolderType, out var requirements))
            {
                if (requirements.RequiredBuildings is { Count: > 0 })
                {
                    BuildingContextModel model = null;
                    buildingsContext = new();

                    foreach (var requiredBuilding in requirements.RequiredBuildings)
                    {
                        if (userBuildingsData.TryGetBuildingsByType(requiredBuilding.BuildingType, out var requiredBuildings))
                        {
                            int level = 0;

                            foreach (var requiredElement in requiredBuildings)
                            {
                                if (requiredElement.Level > level)
                                {
                                    level = requiredElement.Level;
                                }
                            }

                            var building = requiredBuildings.ElementAt(0);


                            model = new BuildingContextModel(building.Template.BuildingContext.BuildingType, building.Template.Name, level, requiredBuilding.RequireLevel);

                            if (requiredBuilding.RequireLevel > level)
                            {
                                notMeetRequirements += 1;
                            }
                        }
                        else
                        {
                            assetLibrary.TryGetBuildingTemplateByType(requiredBuilding.BuildingType, out var buildingTemplate);
                            model = new BuildingContextModel(requiredBuilding.BuildingType, buildingTemplate.Name, 0, requiredBuilding.RequireLevel);
                        }


                        buildingsContext.Add(model);
                    }
                }

                if (requirements.RequiredResources is { Count: > 0 })
                {
                    resourcesContext = new();

                    foreach (var element in requirements.RequiredResources)
                    {
                        if (userResources.GetGameResourceValue(element.ResourceType) < element.RequireAmount)
                        {
                            notMeetRequirements += 1;
                        }

                        assetLibrary.TryGetGameResource(element.ResourceType, out var gameResource);
                        resourcesContext.Add(new ResourceContextModel(element.ResourceType, gameResource.Name, userResources.GetGameResourceValue(element.ResourceType), element.RequireAmount));
                    }
                }
            }

            return new PlaceHolderRequirementsModel("", buildingsContext, resourcesContext, notMeetRequirements == 0);
        }

        internal static void UnlockPlaceHolder(string placeHolderId, IPlaceholderMapTemplate placeholderMapTemplate, UserPlaceHolderData userPlaceHolderData, UserResources userResources)
        {
            var placeHolder = userPlaceHolderData.PlaceHolderDataMap[placeHolderId];

            if (placeholderMapTemplate.TryGetRequirementsByType(placeHolder.PlaceHolderType, out var requirements))
            {
                if (requirements.RequiredResources is { Count: > 0 })
                {
                    foreach (var element in requirements.RequiredResources)
                    {
                        userResources.SetGameResource(element.ResourceType, userResources.GetGameResourceValue(element.ResourceType) - element.RequireAmount);
                    }
                }

                userPlaceHolderData.UpdatePlaceHolderStatus(placeHolder.Id, new PlaceholderModel(placeHolder.Id, placeHolder.TemplateId, PlaceHolderStatus.Unlocked, placeHolder.PlaceHolderType));
            }
        }

        public static string ValueToString(float value)
        {
            value = (float)Math.Round(value);

            if (value < 1000)
                return value.ToString(CultureInfo.InvariantCulture);


            double shortened = value;
            int suffixIndex = -1;

            while (shortened >= 1000)
            {
                shortened /= 1000;
                suffixIndex++;
            }

            double rounded = shortened < 10 ? Math.Round(shortened, 1) : Math.Round(shortened, 0);

            if (rounded >= 1000)
            {
                rounded /= 1000;
                suffixIndex++;
            }

            string number = rounded < 10 ? rounded.ToString("0.#") : rounded.ToString("0");

            string suffix = string.Empty;
            switch (suffixIndex)
            {
                case 0:
                    suffix = "K";
                    break;
                case 1:
                    suffix = "M";
                    break;
                case 2:
                    suffix = "B";
                    break;
                case 3:
                    suffix = "T";
                    break;
                case 4:
                    suffix = "Q";
                    break;
                case 5:
                    suffix = "G";
                    break;
            }

            return $"{number}{suffix}";
        }

        public static float GetBuildingEfficiency(BuildingModel buildingModel, UserBuildingsData userBuildingsData)
        {
            float efficiency = 1;
            if (buildingModel.Template.BuildingContext.ResourcesUse is { Count: > 0 })
            {
                userBuildingsData.TryGetBuildingsByType(BuildingType.MainBuilding, out var mainBuildings);

                foreach (var useResource in buildingModel.Template.BuildingContext.ResourcesUse)
                {
                    float useAmount = 0;
                    float productAmount = 0;

                    userBuildingsData.TryGetBuildingsByUseResourceType(useResource.GameResource.GameResourceType, out var buildingsUse);

                    foreach (var buildingUse in buildingsUse)
                    {
                        foreach (var resourcesUseModel in buildingUse.Template.BuildingContext.ResourcesUse)
                        {
                            if (resourcesUseModel.GameResource.GameResourceType == useResource.GameResource.GameResourceType)
                            {
                                useAmount += GetResourceUse(resourcesUseModel.Amount, buildingUse.Level, buildingUse.Template.BuildingContext.BuildingProduction.LevelMultiplier);
                            }
                        }
                    }

                    if (userBuildingsData.TryGetBuildingsByResourceType(useResource.GameResource.GameResourceType, out var buildingsProduct))
                    {
                        foreach (var buildingProduct in buildingsProduct)
                        {
                            productAmount += GetProductionAmount(mainBuildings.ElementAt(0), buildingProduct, 1);
                        }
                    }

                    efficiency *= (productAmount / useAmount);
                    if (efficiency > 1)
                        efficiency = 1;
                }


                return efficiency;
            }
            else
            {
                return efficiency;
            }
        }

        public static string SecondsToTime(long totalSeconds)
        {
            if (totalSeconds <= 0)
                return "0s";

            long days = totalSeconds / 86400;
            totalSeconds %= 86400;

            long hours = totalSeconds / 3600;
            totalSeconds %= 3600;

            long minutes = totalSeconds / 60;
            long seconds = totalSeconds % 60;

            if (days > 0)
                return $"{days}d {hours}h";

            if (hours > 0)
                return $"{hours}h {minutes}m";

            if (minutes > 0)
                return $"{minutes}m {seconds}s";

            return $"{seconds}s";
        }
    }
}