using System;
using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.GameResources;
using Data.Enum;
using Data.Model;
using Data.Model.Popup;
using UnityEngine;


namespace Data.Utility
{
    public static class DataUtility
    {
        public static bool CanCreateBuilding(UserBuildingsData buildingsData, BuildingTemplate template)
        {
            var buildRequirements = template.BuildingContext.BuildingRequirements;

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

        public static CreateBuildingRequirementsContext GetCreateBuildingRequirementContext(UserResources userResources, UserBuildingsData userBuildingsData, BuildingTemplate buildingTemplate)
        {
            List<BuildingContextModel> buildings = null;

            if (buildingTemplate.BuildingContext.BuildingRequirements is { Count: > 0 })
            {
                buildings = new List<BuildingContextModel>();

                foreach (var element in buildingTemplate.BuildingContext.BuildingRequirements)
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

            return new CreateBuildingRequirementsContext(buildingTemplate.Name, buildingTemplate.Description, buildingTemplate.Icon, buildings, null);
        }

        public static UpgradeBuildingContext GetUpgradeBuildingContext(BuildingModel buildingModel, UserResources userResources, UserBuildingsData userBuildingsData)
        {
            List<BuildingContextModel> buildings = null;
            List<ResourceContextModel> resources = null;

            int notMeetRequirements = 0;

            if (buildingModel.Template.BuildingContext.BuildingRequirements != null)
            {
                buildings = new List<BuildingContextModel>();
                foreach (var element in buildingModel.Template.BuildingContext.BuildingRequirements)
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

                        buildings.Add(new BuildingContextModel(building.Template.BuildingContext.BuildingType, building.Template.Name, building.Level, element.RequiredLevel));

                        if (level < element.RequiredLevel)
                        {
                            notMeetRequirements += 1;
                        }
                    }
                }
            }

            if (buildingModel.Template.BuildingContext.UpgradeCostModel.CostModels != null)
            {
                resources = new List<ResourceContextModel>();
                foreach (var element in buildingModel.Template.BuildingContext.UpgradeCostModel.CostModels)
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

            return new UpgradeBuildingContext(buildingModel.Template.Name, buildingModel.Template.Description, buildingModel.Level, buildingModel.Template.Icon,
                buildingModel.Id, buildingModel.Template.Id, buildings, resources, notMeetRequirements == 0);
        }

        public static float UpgradeResourceCost(float cost, int buildingLevel, float costGrowth)
        {
            return cost * costGrowth * buildingLevel;
        }

        public static float GetMainBuildingBonus(int level, float levelMultiplier)
        {
            return 1 + levelMultiplier * (level - 1);
        }

        public static float GetResourceUse(float baseUse, int buildingLevel, float levelMultiplier)
        {
            return baseUse * levelMultiplier * buildingLevel;
        }

        public static float GetProductionAmount(BuildingModel mainBuildingModel, BuildingModel buildingModel, float useResourcesMultiplier)
        {
            return (buildingModel.Template.BuildingContext.BuildingProduction.Amount + (buildingModel.Level - 1) * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier) *
                   GetMainBuildingBonus(mainBuildingModel.Level, mainBuildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier) * useResourcesMultiplier;
        }


        public static float GetResourceMaxCapacity(GameResourcesTemplate resource, ICollection<BuildingModel> warehouseModels)
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

        public static int GetCapacity(GameResourcesTemplate resource, int warehouseLevel)
        {
            return Mathf.RoundToInt(resource.BaseCapacity + GetWarehouseBonus(warehouseLevel) * resource.StorageFactor);
        }

        private static int GetWarehouseBonus(int level)
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
    }
}