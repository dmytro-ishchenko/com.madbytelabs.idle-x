using Data.Events;
using Data.Model;

namespace Data.Factory
{
    internal interface IBuildingFactory
    {
        bool TryCreateBuilding(UserData userData, BuildingProcessEventArgs args, out BuildingModel buildingModel);
        bool TryUpgradeBuilding(UserData userData, BuildingProcessEventArgs args, out BuildingModel buildingModel);
        bool TryDismantleBuilding(UserData userData, BuildingProcessEventArgs args, out BuildingModel buildingModel);
    }
}