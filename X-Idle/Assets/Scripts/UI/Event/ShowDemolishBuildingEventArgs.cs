using Common.Pattern.BobbleEvent;
using Data.Model;

namespace UI.Event
{
    public class ShowDemolishBuildingEventArgs : IEventArgs
    {
        public ShowDemolishBuildingEventArgs(BuildingModel buildingModel)
        {
            BuildingModel = buildingModel;
        }

        public BuildingModel BuildingModel { get; }
    }
}