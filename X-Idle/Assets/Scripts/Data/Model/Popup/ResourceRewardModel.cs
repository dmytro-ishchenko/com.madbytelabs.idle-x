using UnityEngine;

namespace Data.Model.Popup
{
    public struct ResourceRewardModel
    {
        public ResourceRewardModel(Sprite icon, string resourceName, float amount, bool addAllResources)
        {
            Icon = icon;
            ResourceName = resourceName;
            Amount = amount;
            AddAllResources = addAllResources;
        }

        public Sprite Icon { get; }
        public string ResourceName { get; }
        public float Amount { get; }
        public bool AddAllResources { get; }
    }
}