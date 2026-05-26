using UnityEngine;

namespace Data.Model.Popup
{
    public struct ResourceInfoContext : IShowPopupContext
    {
        public ResourceInfoContext(string resourceName, Sprite resourceIcon, string resourceDescription, string productionBuildingName, Sprite productionBuildingIcon, float currentProduction, float currentAmount, float storageCapacity, float currentUse)
        {
            ResourceName = resourceName;
            ResourceIcon = resourceIcon;
            ResourceDescription = resourceDescription;
            ProductionBuildingName = productionBuildingName;
            ProductionBuildingIcon = productionBuildingIcon;
            CurrentProduction = currentProduction;
            StorageCapacity = storageCapacity;
            CurrentUse = currentUse;
            CurrentAmount = currentAmount;
        }

        public string ResourceName { get; }
        public Sprite ResourceIcon { get; }
        public string ResourceDescription { get; }
        public string ProductionBuildingName { get; }
        public Sprite ProductionBuildingIcon { get; }
        public float CurrentProduction { get; }
        public float CurrentUse { get; }
        public float StorageCapacity { get; }
        public float CurrentAmount { get; }
    }
}