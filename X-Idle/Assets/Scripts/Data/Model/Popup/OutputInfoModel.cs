using UnityEngine;

namespace Data.Model.Popup
{
    public struct OutputInfoModel
    {
        public OutputInfoModel(string blockName, string resourceName, Sprite icon, OutputElement resource, OutputElement storage, bool showAsBonus)
        {
            BlockName = blockName;
            ResourceName = resourceName;
            Icon = icon;
            Resource = resource;
            Storage = storage;
            ShowAsBonus = showAsBonus;
        }


        public string BlockName { get; }
        public string ResourceName { get; }
        public Sprite Icon { get; }
        public OutputElement Resource { get; }
        public OutputElement Storage { get; }
        public bool ShowAsBonus { get; }
    }
}