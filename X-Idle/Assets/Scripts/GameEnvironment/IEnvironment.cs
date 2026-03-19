using System;
using Data.Events;

namespace GameEnvironment
{
    public interface IEnvironment
    {
        event Action<BuildingEventArgs> OnCreateBuildingRequest;
        event Action<BuildingEventArgs> OnUpgradeBuildingRequest;
    }
}