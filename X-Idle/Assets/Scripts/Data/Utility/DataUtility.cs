using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.GameResources;
using Data.ContentLibrary.Templates.Placeholder;
using Data.Enum;
using Data.Model;
using Data.Model.Popup;
using Data.Persistent;
using UnityEngine;


namespace Data.Utility
{
    public static class DataUtility
    {
        public static bool CanCreateBuilding(UserBuildingsData buildingsData, BuildingTemplate template)
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

        public static CreateBuildingRequirementsContext GetCreateBuildingRequirementContext(UserResources userResources, UserBuildingsData userBuildingsData, BuildingTemplate buildingTemplate)
        {
            List<BuildingContextModel> buildings = null;

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

            return new CreateBuildingRequirementsContext(buildingTemplate.Name, buildingTemplate.Description, buildingTemplate.Icon, buildings, null);
        }

        public static UpgradeBuildingContext GetUpgradeBuildingContext(BuildingModel buildingModel, IAssetLibrary assetLibrary, UserResources userResources, UserBuildingsData userBuildingsData)
        {
            List<BuildingContextModel> buildings = null;
            List<ResourceContextModel> resources = null;

            int notMeetRequirements = 0;

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

        public static PlaceHolderRequirementsModel GetPlaceHolderRequirements(string placeHolderId,
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

        public static void UnlockPlaceHolder(string placeHolderId, IPlaceholderMapTemplate placeholderMapTemplate, UserPlaceHolderData userPlaceHolderData, UserResources userResources)
        {
            var placeHolder = userPlaceHolderData.PlaceHolderDataMap[placeHolderId];

            if (placeholderMapTemplate.TryGetRequirementsByType(placeHolder.PlaceHolderType, out var requirements))
            {
                if (requirements.RequiredResources is { Count: > 0 })
                {
                    foreach (var element in requirements.RequiredResources)
                    {
                        userResources.SetGameResource(element.ResourceType, element.RequireAmount);
                    }
                }

                userPlaceHolderData.UpdatePlaceHolderStatus(placeHolder.Id, new PlaceholderModel(placeHolder.Id, PlaceHolderStatus.Unlocked, placeHolder.PlaceHolderType));
            }
        }
    }
}