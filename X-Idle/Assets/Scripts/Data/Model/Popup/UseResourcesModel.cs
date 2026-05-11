using UnityEngine;

namespace Data.Model.Popup
{
    public struct UseResourcesModel
    {
        public UseResourcesModel(string resourceName, Sprite icon, string currentUse, string nextUse)
        {
            ResourceName = resourceName;
            Icon = icon;
            CurrentUse = currentUse;
            NextUse = nextUse;
        }

        public string ResourceName { get; }
        public Sprite Icon { get; }
        public string CurrentUse { get; }
        public string NextUse { get; }
    }
}