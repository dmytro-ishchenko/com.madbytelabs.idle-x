using System.Collections.Generic;

namespace Data.Model.Popup
{
    public class PlaceHolderRequirementsModel
    {
        public PlaceHolderRequirementsModel(string description, IList<BuildingContextModel> buildings, IList<ResourceContextModel> resources, bool canUpgrade)
        {
            Description = description;
            Buildings = buildings;
            Resources = resources;
            CanUpgrade = canUpgrade;
        }

        public string Description { get; }
        public IList<BuildingContextModel> Buildings { get; }
        public IList<ResourceContextModel> Resources { get; }
        public bool CanUpgrade { get; }
    }
}