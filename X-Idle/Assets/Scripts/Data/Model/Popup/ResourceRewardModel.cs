using UnityEngine;

namespace Data.Model.Popup
{
    public struct ResourceRewardModel
    {
        public ResourceRewardModel(Sprite icon, string resourceName, float amount)
        {
            Icon = icon;
            ResourceName = resourceName;
            Amount = amount;
          
        }

        public Sprite Icon { get; }
        public string ResourceName { get; }
        public float Amount { get; }
  
    }
}