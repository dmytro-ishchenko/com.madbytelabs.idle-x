using UnityEngine;

namespace Data.Model.Popup
{
    public struct StorageReachLimitModel
    {
        public StorageReachLimitModel(Sprite icon, string resourceName)
        {
            Icon = icon;
            ResourceName = resourceName;
        }

        public Sprite Icon { get; }
        public string ResourceName { get; }
    }
}