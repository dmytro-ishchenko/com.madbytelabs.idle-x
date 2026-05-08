using System.Collections.Generic;

namespace Data.Model.Popup
{
    public struct RequirementsModel
    {
        public RequirementsModel(IList<BuildingContextModel> buildings, IList<ResourceContextModel> resources)
        {
            Buildings = buildings;
            Resources = resources;
        }


        public IList<BuildingContextModel> Buildings { get; }
        public IList<ResourceContextModel> Resources { get; }
    }
}