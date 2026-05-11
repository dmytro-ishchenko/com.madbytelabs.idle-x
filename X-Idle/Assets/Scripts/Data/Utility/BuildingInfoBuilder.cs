using System.Collections.Generic;
using Data.ContentLibrary;
using Data.Enum;
using Data.Model;
using Data.Model.Popup;
using UnityEngine;

namespace Data.Utility
{
    internal static class BuildingInfoBuilder
    {
        public static SelectBuildingInfo GetBuildingInfo(string buildingId, IAssetLibrary assetLibrary,
            UserBuildingsData userBuildingsData)
        {
            userBuildingsData.TryGetBuildingModel(buildingId, out BuildingModel buildingModel);

            List<InfoElementModel> effects = null;
            InfoElementModel storage = null;
            InfoElementModel condition = null;

            var context = GetBuildingInfo(buildingModel, assetLibrary, userBuildingsData, buildingModel.Level);
            effects = context.effects;
            storage = context.storage;

            int conditionValue = (int)(DataUtility.GetBuildingEfficiency(buildingModel, userBuildingsData) * 100);
            condition = new InfoElementModel($"{conditionValue}%");


            return new SelectBuildingInfo(buildingModel.Template.Name, buildingModel.Template.Icon, buildingModel.Level,
                buildingModel.Template.Description, effects, storage, condition);
        }

        public static (List<InfoElementModel>effects, InfoElementModel storage) GetBuildingInfo(
            BuildingModel buildingModel, IAssetLibrary assetLibrary, UserBuildingsData userBuildingsData, int level)
        {
            List<InfoElementModel> effects = null;
            InfoElementModel storage = null;

            switch (buildingModel.Template.BuildingContext.BuildingType)
            {
                case BuildingType.MainBuilding:
                    effects = new();
                    effects.Add(new InfoElementModel(
                        buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Icon,
                        $"{buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Name}"
                        , $"{level * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * 100}%"));
                    break;
                case BuildingType.Warehouse:
                    effects = new();
                    effects.Add(new InfoElementModel(
                        buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Icon,
                        $"{buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Name}"
                        , $"{DataUtility.ValueToString(DataUtility.GetWarehouseBonus(level))}"));
                    break;
                default:
                    effects = new();
                    effects.Add(new InfoElementModel(
                        buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Icon,
                        $"{buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Name}"
                        , $"+{DataUtility.ValueToString(buildingModel.Template.BuildingContext.BuildingProduction.Amount * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * level * 60)} /m"));

                    if (buildingModel.Template.BuildingContext.ResourcesUse is { Count: > 0 })
                    {
                        foreach (var resourcesUseModel in buildingModel.Template.BuildingContext.ResourcesUse)
                        {
                            effects.Add(new InfoElementModel(resourcesUseModel.GameResource.Icon,
                                $"{resourcesUseModel.GameResource.Name}"
                                , $"-{DataUtility.ValueToString(resourcesUseModel.Amount * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * level * 60)} /m"));
                        }
                    }


                    assetLibrary.TryGetGameResource(GameResourceType.Storage, out var storageResource);

                    var warehouseLevel = 0;

                    if (userBuildingsData.TryGetBuildingsByType(BuildingType.Warehouse, out var warehouses))
                    {
                        foreach (var warehouse in warehouses)
                        {
                            if (warehouse.Level >= warehouseLevel)
                                warehouseLevel = warehouse.Level;
                        }
                    }

                    var value =
                        buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.BaseCapacity +
                        DataUtility.GetCapacity(
                            buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate,
                            warehouseLevel);

                    storage = new InfoElementModel(storageResource.Icon, storageResource.Name,
                        DataUtility.ValueToString(value));
                    break;
            }


            return (effects, storage);
        }

        public static OutputInfoModel GetProductionInfoModel(BuildingModel buildingModel, IAssetLibrary assetLibrary,
            UserBuildingsData userBuildingsData)
        {
            OutputElement resource = null, storage = null;

            string blockName = string.Empty, current = string.Empty, next = string.Empty;
            bool showAsBonus = false;

            string resourceName = string.Empty;
            Sprite icon = buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Icon;
            switch (buildingModel.Template.BuildingContext.BuildingType)
            {
                case BuildingType.MainBuilding:
                    showAsBonus = true;
                    blockName = "Bonus";
                    current =
                        $"{buildingModel.Level * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * 100}%";
                    next =
                        $"{(buildingModel.Level + 1) * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * 100}%";
                    resource = new OutputElement(current, next);
                    break;
                case BuildingType.Warehouse:
                    showAsBonus = true;
                    blockName = "Bonus";
                    current = $"{DataUtility.ValueToString(DataUtility.GetWarehouseBonus(buildingModel.Level))}";
                    next = $"{DataUtility.ValueToString(DataUtility.GetWarehouseBonus(buildingModel.Level + 1))}";
                    storage = new OutputElement(current, next);
                    break;
                default:
                    blockName = "Output";
                    resourceName = buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Name;
                    current =
                        $"+{DataUtility.ValueToString(buildingModel.Template.BuildingContext.BuildingProduction.Amount * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * buildingModel.Level * 60)}/m";
                    next =
                        $"+{DataUtility.ValueToString(buildingModel.Template.BuildingContext.BuildingProduction.Amount * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * (buildingModel.Level + 1) * 60)}/m";

                    resource = new OutputElement(current, next);

                    var warehouseLevel = 0;

                    if (userBuildingsData.TryGetBuildingsByType(BuildingType.Warehouse, out var warehouses))
                    {
                        foreach (var warehouse in warehouses)
                        {
                            if (warehouse.Level >= warehouseLevel)
                                warehouseLevel = warehouse.Level;
                        }
                    }

                    var value =
                        buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.BaseCapacity +
                        DataUtility.GetCapacity(
                            buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate,
                            warehouseLevel);


                    storage = new OutputElement(DataUtility.ValueToString(value), string.Empty);


                    break;
            }

            return new OutputInfoModel(blockName, resourceName, icon, resource, storage, showAsBonus);
        }

        public static IList<UseResourcesModel> GetUseResourcesModels(BuildingModel buildingModel, IAssetLibrary assetLibrary, UserBuildingsData userBuildingsData)
        {
            if (buildingModel.Template.BuildingContext.ResourcesUse is { Count: > 0 })
            {
                IList<UseResourcesModel> models = new List<UseResourcesModel>();
                foreach (var resource in buildingModel.Template.BuildingContext.ResourcesUse)
                {
                    float currentUse = resource.Amount * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * buildingModel.Level * 60;
                    float nextUse = resource.Amount * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * (buildingModel.Level + 1) * 60;

                    models.Add(new UseResourcesModel(resource.GameResource.Name, resource.GameResource.Icon, $"-{DataUtility.ValueToString(currentUse)}/m", $"-{DataUtility.ValueToString(nextUse)}/m"));
                }

                return models;
            }

            return null;
        }
    }
}