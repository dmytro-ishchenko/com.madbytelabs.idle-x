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
        UserResources UserResources { get; }
        UserBuildingsData UserBuildingsData { get; }
        IList<IBuildingTemplate> GetAvailableBuilding();
        void BuildingProcess(BuildingProcessEventArgs args);
        event Action<UserResources> OnUserResourcesChanged;
        event Action<BuildingModel> OnBuildingCreated;
        event Action<ActionErrorModel> OnActionError;
    }
}