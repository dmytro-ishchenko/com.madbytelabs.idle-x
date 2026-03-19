using Common.Pattern.BobbleEvent;
using Data.Model;
using UnityEngine;

namespace Data.Interface
{
    public interface IBuildingView : IMonoNode
    {
        GameObject GameObject { get; }

        BuildingModel Model { get; }
        void Init(BuildingModel model);
    }
}