using Common.Pattern.BobbleEvent;
using Data.Enum;

namespace Data.Events
{
    public class BuildingProcessEventArgs : IEventArgs
    {
        public BuildingProcessEventArgs(string buildingId, BuildingActionType buildingRequestType)
        {
            BuildingId = buildingId;
            BuildingActionType = buildingRequestType;
        }

        public string BuildingId { get; }
        public BuildingActionType BuildingActionType { get; }
    }
}