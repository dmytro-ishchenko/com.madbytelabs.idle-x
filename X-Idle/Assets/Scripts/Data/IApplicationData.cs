using System.Collections.Generic;
using Data.ContentLibrary;
using Data.Interface;

namespace Data
{
    public interface IApplicationData
    {
        IAssetLibrary AssetLibrary { get; }
        IList<IBuildingTemplate> GetAvailableBuilding();
    }
}