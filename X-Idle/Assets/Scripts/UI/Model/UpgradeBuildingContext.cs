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

        public BuildingModel BuildingModel { get; }

        public UserResources UserResources { get; }

        public UserBuildingsData UserBuildingsData { get; }
    }
}