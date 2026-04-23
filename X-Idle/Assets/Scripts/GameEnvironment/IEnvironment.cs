using System;
using Data.Events;

namespace GameEnvironment
{
    public interface IEnvironment
    {
        event Action<BuildingRequestEventArgs> OnBuildingActionRequest;
       
    }
}