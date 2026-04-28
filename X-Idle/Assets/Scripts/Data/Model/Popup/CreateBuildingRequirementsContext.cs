using System.Collections.Generic;
using UnityEngine;

namespace Data.Model.Popup
{
    public struct CreateBuildingRequirementsContext : IShowPopupContext
    {
        public CreateBuildingRequirementsContext(string name, string description, Sprite icon, List<BuildingContextModel> buildings, List<ResourceContextModel> resources)
        {
            Name = name;
            Icon = icon;
            Buildings = buildings;
            Resources = resources;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
        public Sprite Icon { get; }

        public List<BuildingContextModel> Buildings { get; }
        public List<ResourceContextModel> Resources { get; }
    }
}