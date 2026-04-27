using Data.ContentLibrary.Templates;
using Data.Model;

namespace UI.Model
{
    internal struct CreateBuildingErrorContext
    {
        public CreateBuildingErrorContext(UserResources userResources, UserBuildingsData userBuildingsData, BuildingTemplate buildingTemplate)
        {
            UserResources = userResources;
            UserBuildingsData = userBuildingsData;
            BuildingTemplate = buildingTemplate;
        }


        public UserResources UserResources { get; }

        public UserBuildingsData UserBuildingsData { get; }
        public BuildingTemplate BuildingTemplate { get; }
    }
}