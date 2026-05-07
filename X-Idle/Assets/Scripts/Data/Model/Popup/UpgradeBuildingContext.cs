using System.Collections.Generic;
using UnityEngine;

namespace Data.Model.Popup
{
    public struct UpgradeBuildingContext : IShowPopupContext
    {
        public UpgradeBuildingContext(string name, string description, int level, Sprite icon, string id, string templateId, IList<InfoElementModel> currentInfoModels,
            IList<InfoElementModel> nextInfoModels, IList<BuildingContextModel> buildings, IList<ResourceContextModel> resources, bool canUpgrade)
        {
            Name = name;
            Description = description;
            Level = level;
            Icon = icon;
            Id = id;
            TemplateId = templateId;
            Buildings = buildings;
            Resources = resources;
            CanUpgrade = canUpgrade;
            CurrentInfoModels = currentInfoModels;
            NextInfoModels = nextInfoModels;
        }

        public string Name { get; }
        public string Description { get; }
        public int Level { get; }
        public Sprite Icon { get; }
        public string Id { get; }
        public string TemplateId { get; }

        public IList<InfoElementModel> CurrentInfoModels { get; }
        public IList<InfoElementModel> NextInfoModels { get; }
        public IList<BuildingContextModel> Buildings { get; }
        public IList<ResourceContextModel> Resources { get; }
        public bool CanUpgrade { get; }
    }
}