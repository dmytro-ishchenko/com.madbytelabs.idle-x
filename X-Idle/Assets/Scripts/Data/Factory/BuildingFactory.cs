using Data.ContentLibrary;
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
                        buildingModel = building;

                        return true;
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
                        userData.UserResources.SetGameResource(model.GameResourceType,
                            userData.UserResources.GetGameResourceValue(model.GameResourceType) -
                            model.Cost * model.CostGrowth * building.Level);
                    }

                    building.SetLevel(building.Level + 1);
                    buildingModel = building;
                    return true;
                }
            }

            buildingModel = null;
            return false;
        }
    }
}