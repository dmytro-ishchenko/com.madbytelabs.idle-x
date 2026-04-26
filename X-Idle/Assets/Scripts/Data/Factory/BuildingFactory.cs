using System.Collections.Generic;
using System.Linq;
using Data.ContentLibrary;
using Data.Events;
using Data.Model;

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
                if (userData.UserBuildingsData.TryGetBuildingModel(args.BuildingId, out var building))
                {
                    var buildRequirements = buildingTemplate.BuildingContext.BuildingRequirements;

                    if (buildRequirements is { Count: > 0 })
                    {
                        foreach (var element in buildRequirements)
                        {
                            m_assetLibrary.TryGetBuildingTemplate(element.TemplateId, out var requiredTemplate);

                            if (!userData.UserBuildingsData.TryGetBuildingsByType(requiredTemplate.BuildingContext.BuildingType, out var requiredBuildings))
                            {
                                buildingModel = null;
                                return false;
                            }

                            foreach (var model in requiredBuildings)
                            {
                                if (model.Level >= element.Level)
                                {
                                    userData.UserBuildingsData.CreateBuilding(building.Id, buildingTemplate);
                                    buildingModel = building;
                                    return true;
                                }
                            }

                            buildingModel = null;
                            return false;
                        }
                    }
                    else
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
                }
            }

            buildingModel = null;
            return false;
        }
    }
}