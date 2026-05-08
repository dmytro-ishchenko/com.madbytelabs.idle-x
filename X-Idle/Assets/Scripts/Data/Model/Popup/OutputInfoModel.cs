using UnityEngine;

namespace Data.Model.Popup
{
    public struct OutputInfoModel
    {
        public OutputInfoModel( string resourceName, Sprite icon, OutputElement resource, OutputElement storage)
        {
            ResourceName = resourceName;
            Icon = icon;
            Resource = resource;
            Storage = storage;
        }

        public string ResourceName { get; }
        public Sprite Icon { get; }

        public OutputElement Resource { get; }
        public OutputElement Storage { get; }
    }
}