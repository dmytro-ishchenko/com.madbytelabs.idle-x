using System.Collections.Generic;
using Data.Interface;

namespace Data.Model.Popup
{
    public struct CreateBuildingContext : IShowPopupContext
    {
        public CreateBuildingContext(string placeholderId, IList<IBuildingTemplate> buildings)
        {
            PlaceholderId = placeholderId;
            Buildings = buildings;
        }

        public string PlaceholderId { get; }
        public IList<IBuildingTemplate> Buildings { get; }
    }
}