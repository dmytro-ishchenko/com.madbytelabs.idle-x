using Common.Pattern.BobbleEvent;
using Data.Model;

namespace UI.Event
{
    public class ShowUpgradeBuildingEventArgs : IEventArgs
    {
        public ShowUpgradeBuildingEventArgs(BuildingModel buildingModel)
        {
            BuildingModel = buildingModel;
        }

        public BuildingModel BuildingModel { get; }
    }
}