using System.Collections.Generic;

namespace Data.Model.Popup
{
    public struct BlockedPlaceHolderContext
    {
        public BlockedPlaceHolderContext(string id, string description, IList<BuildingContextModel> buildings, IList<ResourceContextModel> resources, bool canUpgrade)
        {
            Id = id;
            Description = description;
            Buildings = buildings;
            Resources = resources;
            CanUpgrade = canUpgrade;
        }

        public string Id { get; }
        public string Description { get; }
        public IList<BuildingContextModel> Buildings { get; }
        public IList<ResourceContextModel> Resources { get; }
        public bool CanUpgrade { get; }
    }
}