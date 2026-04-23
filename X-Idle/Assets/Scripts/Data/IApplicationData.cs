using System;
using System.Collections.Generic;
using Data.ContentLibrary;
using Data.Events;
using Data.Interface;

namespace Data
{
    public interface IApplicationData
    {
        void InitApplicationData(Action complete);
        IAssetLibrary AssetLibrary { get; }
        IList<IBuildingTemplate> GetAvailableBuilding();
        void BuildingProcess(BuildingProcessEventArgs args);
    }
}