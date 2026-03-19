using System.Collections.Generic;
using Data.Interface;

namespace UI.Model
{
    public struct CreateBuildingContext
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