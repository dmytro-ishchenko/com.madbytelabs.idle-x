using System;
using System.Collections.Generic;
using Data.Events;
using Data.Interface;
using Data.Model;

namespace Data
{
    public interface IApplicationData
    {
        void InitApplicationData(Action complete);
        IList<BuildingModel> UserBuildings { get; }

        IList<IBuildingTemplate> GetAvailableBuilding();
        void BuildingProcess(BuildingProcessEventArgs args);

        event Action<BuildingModel> OnBuildingCreated;
    }
}