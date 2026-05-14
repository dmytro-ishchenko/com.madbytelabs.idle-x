using System.Collections.Generic;
using Data.Interface;

namespace Data.Model.Popup
{
    public struct CreateBuildingContext : IShowPopupContext
    {
        public CreateBuildingContext(string placeholderId, IList<IBuildingTemplate> buildings, IApplicationData applicationData)
        {
            PlaceholderId = placeholderId;
            Buildings = buildings;
            ApplicationData = applicationData;
        }

        public string PlaceholderId { get; }
        public IList<IBuildingTemplate> Buildings { get; }
        public IApplicationData ApplicationData { get; }
    }
}