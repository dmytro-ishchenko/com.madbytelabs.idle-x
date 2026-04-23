using Common.Pattern.BobbleEvent;
using Data.Enum;

namespace Data.Events
{
    public class BuildingProcessEventArgs : IEventArgs
    {
        public BuildingProcessEventArgs(string buildingId, string templateId, BuildingActionType buildingRequestType)
        {
            BuildingId = buildingId;
            TemplateId = templateId;
            BuildingActionType = buildingRequestType;
        }

        public string BuildingId { get; }
        public string TemplateId { get; }
        public BuildingActionType BuildingActionType { get; }
    }
}