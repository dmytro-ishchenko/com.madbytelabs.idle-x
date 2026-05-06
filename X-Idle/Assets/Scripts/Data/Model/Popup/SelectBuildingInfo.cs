using System.Collections.Generic;
using UnityEngine;

namespace Data.Model.Popup
{
    public class SelectBuildingInfo : IShowPopupContext
    {
        public SelectBuildingInfo(string name, Sprite icon, int level, string description, List<InfoElementModel> effects, List<InfoElementModel> storageList, InfoElementModel condition)
        {
            Name = name;
            Level = level;
            Description = description;
            Effects = effects;
            StorageList = storageList;
            Condition = condition;
            Icon = icon;
        }

        public string Name { get; }

        public Sprite Icon { get; }
        public int Level { get; }
        public string Description { get; }
        public List<InfoElementModel> Effects { get; }
        public List<InfoElementModel> StorageList { get; }
        public InfoElementModel Condition{ get; }
    }
}