using Common.Pattern.BobbleEvent;
using Data.Model;

namespace UI.Event
{
    public class ShowDismantleBuildingEventArgs : IEventArgs
    {
        public ShowDismantleBuildingEventArgs(BuildingModel buildingModel)
        {
            BuildingModel = buildingModel;
        }

        public BuildingModel BuildingModel { get; }
    }
}