using Data.ContentLibrary.Templates;
using Data.Enum;

namespace Data.Model
{
    public struct ActionErrorModel
    {
        public ActionErrorModel(ActionErrorType actionErrorType, BuildingTemplate buildingTemplate)
        {
            ActionErrorType = actionErrorType;
            BuildingTemplate = buildingTemplate;
        }

        public ActionErrorType ActionErrorType { get; }
        public BuildingTemplate BuildingTemplate { get; }
    }
}