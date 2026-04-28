using System;
using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary.Templates;
using Data.Model;
using Data.Model.Popup;


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
            return (float)Math.Round(cost * costGrowth * buildingLevel);
        }

        public static float GetMainBuildingBonus(int level, float levelMultiplier)
        {
            return 1 + levelMultiplier * (level - 1);
        }
    }
}