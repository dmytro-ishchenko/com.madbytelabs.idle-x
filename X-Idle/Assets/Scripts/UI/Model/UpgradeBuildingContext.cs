using Data.Model;

namespace UI.Model
{
    public struct UpgradeBuildingContext
    {
        public UpgradeBuildingContext(UserResources userResources, UserBuildingsData userBuildingsData, BuildingModel buildingModel)
        {
            UserResources = userResources;
            UserBuildingsData = userBuildingsData;
            BuildingModel = buildingModel;
        }

        public readonly BuildingModel BuildingModel { get; }

        public readonly UserResources UserResources { get; }

        public readonly UserBuildingsData UserBuildingsData { get; }
    }
}