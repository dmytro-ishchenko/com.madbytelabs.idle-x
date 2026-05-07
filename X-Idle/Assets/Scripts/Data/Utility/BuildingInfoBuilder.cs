using System.Collections.Generic;
using Data.ContentLibrary;
using Data.Enum;
using Data.Model;
using Data.Model.Popup;

namespace Data.Utility
{
    internal static class BuildingInfoBuilder
    {
        public static SelectBuildingInfo GetBuildingInfo(string buildingId, IAssetLibrary assetLibrary, UserBuildingsData userBuildingsData)
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


            return new SelectBuildingInfo(buildingModel.Template.Name, buildingModel.Template.Icon, buildingModel.Level, buildingModel.Template.Description, effects, storage, condition);
        }

        public static (List<InfoElementModel>effects, InfoElementModel storage) GetBuildingInfo(BuildingModel buildingModel, IAssetLibrary assetLibrary, UserBuildingsData userBuildingsData, int level)
        {
            List<InfoElementModel> effects = null;
            InfoElementModel storage = null;

            switch (buildingModel.Template.BuildingContext.BuildingType)
            {
                case BuildingType.MainBuilding:
                    effects = new();
                    effects.Add(new InfoElementModel(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Icon,
                        $"{buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Name}"
                        , $"{level * buildingModel.Template.BuildingContext.BuildingProduction.LevelMultiplier * 100}%"));
                    break;
                case BuildingType.Warehouse:
                    effects = new();
                    effects.Add(new InfoElementModel(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Icon,
                        $"{buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Name}"
                        , $"{DataUtility.ValueToString(DataUtility.GetWarehouseBonus(level))}"));
                    break;
                default:
                    effects = new();
                    effects.Add(new InfoElementModel(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Icon,
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

                    var value = buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.BaseCapacity +
                                DataUtility.GetCapacity(buildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate, warehouseLevel);

                    storage = new InfoElementModel(storageResource.Icon, storageResource.Name, DataUtility.ValueToString(value));
                    break;
            }


            return (effects, storage);
        }
    }
}