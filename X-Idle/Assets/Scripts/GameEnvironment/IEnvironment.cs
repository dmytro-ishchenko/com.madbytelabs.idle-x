using System;
using Data.Events;
using UnityEngine;

namespace GameEnvironment
{
    public interface IEnvironment
    {
        event Action<BuildingRequestEventArgs> OnBuildingActionRequest;
        Transform GetPlaceholderTransform(string id);
    }
}