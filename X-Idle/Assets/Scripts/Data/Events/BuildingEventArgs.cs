using Common.Pattern.BobbleEvent;
using Data.Enum;
using Data.Model;

namespace Data.Events
{
    public struct BuildingEventArgs : IEventArgs
    {
        public BuildingEventArgs(BuildingModel buildingModel, BuildingActionType buildingActionType)
        {
            BuildingModel = buildingModel;
            BuildingActionType = buildingActionType;
        }

        public BuildingModel BuildingModel { get; }
        public BuildingActionType BuildingActionType { get; }
    }
}