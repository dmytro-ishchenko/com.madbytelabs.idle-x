using Data.ContentLibrary;
using Data.Enum;
using Data.Events;
using Data.Model;
using Data.Utility;

namespace Data.Factory
{
    internal class BuildingFactory : IBuildingFactory
    {
        public BuildingFactory(IAssetLibrary assetLibrary)
        {
            m_assetLibrary = assetLibrary;
        }

        private readonly IAssetLibrary m_assetLibrary;

        public bool TryCreateBuilding(UserData userData, BuildingProcessEventArgs args, out BuildingModel buildingModel)
        {
            if (m_assetLibrary.TryGetBuildingTemplate(args.TemplateId, out var buildingTemplate))
            {
                if (DataUtility.CanCreateBuilding(userData.UserBuildingsData, buildingTemplate))
                {
                    if (userData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out var building))
                    {
                        userData.UserBuildingsData.CreateBuilding(building.Id, buildingTemplate);

                        if (userData.UserPlaceHolderData.PlaceHolderDataMap.TryGetValue(building.Id, out var placeHolder))
                        {
                            userData.UserPlaceHolderData.UpdatePlaceHolderStatus(building.Id, new PlaceholderModel(placeHolder.Id, PlaceHolderStatus.Occupied, placeHolder.PlaceHolderType));

                            buildingModel = building;

                            return true;
                        }
                    }
                }
            }

            buildingModel = null;
            return false;
        }

        public bool TryUpgradeBuilding(UserData userData, BuildingProcessEventArgs args, out BuildingModel buildingModel)
        {
            if (m_assetLibrary.TryGetBuildingTemplate(args.TemplateId, out var buildingTemplate))
            {
                if (userData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out var building))
                {
                    foreach (var model in buildingTemplate.BuildingContext.UpgradeCostModel.CostModels)
                    {
                        userData.UserResources.SetGameResource(model.GameResource.GameResourceType,
                            userData.UserResources.GetGameResourceValue(model.GameResource.GameResourceType) - DataUtility.UpgradeResourceCost(model.Cost, building.Level, model.CostGrowth));
                    }

                    userData.UserResources.InvokeUpdateResources();

                    building.SetLevel(building.Level + 1);
                    buildingModel = building;
                    return true;
                }
            }

            buildingModel = null;
            return false;
        }

        public bool TryDismantleBuilding(UserData userData, BuildingProcessEventArgs args, out BuildingModel buildingModel)
        {
            if (m_assetLibrary.TryGetBuildingTemplateByType(BuildingType.DestroyedBuilding, out var template))
            {
                if (userData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out var building))
                {
                    var level = building.Level;


                    for (int i = 1; i < level; i++)
                    {
                        foreach (var element in building.Template.BuildingContext.UpgradeCostModel.CostModels)
                        {
                            userData.UserResources.SetGameResource(element.GameResource.GameResourceType, 
                                userData.UserResources.GetGameResourceValue(element.GameResource.GameResourceType) +
                                element.Cost * element.CostGrowth * i * 0.5f);
                        }
                    }


                    userData.UserBuildingsData.DeleteBuilding(building.Id, template);
                    building.SetLevel(1);
                    if (userData.UserPlaceHolderData.PlaceHolderDataMap.TryGetValue(building.Id, out var placeHolder))
                    {
                        userData.UserPlaceHolderData.UpdatePlaceHolderStatus(placeHolder.Id, new PlaceholderModel(building.Id, PlaceHolderStatus.Unlocked, placeHolder.PlaceHolderType));

                        buildingModel = building;
                        return true;
                    }
                }
            }

            buildingModel = null;
            return false;
        }
    }
}