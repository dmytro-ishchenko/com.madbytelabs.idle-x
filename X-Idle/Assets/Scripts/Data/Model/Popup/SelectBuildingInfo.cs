using System.Collections.Generic;
using UnityEngine;

namespace Data.Model.Popup
{
    public class SelectBuildingInfo : IShowPopupContext
    {
        public SelectBuildingInfo(string name, Sprite icon, int level, string description, List<InfoElementModel> effects, InfoElementModel storage, ConditionModel condition)
        {
            Name = name;
            Level = level;
            Description = description;
            Effects = effects;
            Storage = storage;
            Condition = condition;
            Icon = icon;
        }

        public string Name { get; }

        public Sprite Icon { get; }
        public int Level { get; }
        public string Description { get; }
        public List<InfoElementModel> Effects { get; }
        public InfoElementModel Storage { get; }
        public ConditionModel Condition { get; }
    }
}