using System.Collections.Generic;
using UnityEngine;

namespace Data.Model.Popup
{
    public struct UpgradeBuildingContext : IShowPopupContext
    {
        public UpgradeBuildingContext(string name, string description, int level, Sprite icon, string id, string templateId, OutputInfoModel outputModel, IList<UseResourcesModel> useResourcesModels, RequirementsModel requirementsModel, bool canUpgrade)
        {
            Name = name;
            Description = description;
            Level = level;
            Icon = icon;
            Id = id;
            TemplateId = templateId;

            CanUpgrade = canUpgrade;
            RequirementsModel = requirementsModel;
            OutputModel = outputModel;
            UseResourcesModels = useResourcesModels;
        }

        public string Name { get; }
        public string Description { get; }
        public int Level { get; }
        public Sprite Icon { get; }
        public string Id { get; }
        public string TemplateId { get; }
        public OutputInfoModel OutputModel { get; }
        public IList<UseResourcesModel> UseResourcesModels { get; }

        public RequirementsModel RequirementsModel { get; }
        public bool CanUpgrade { get; }
    }
}