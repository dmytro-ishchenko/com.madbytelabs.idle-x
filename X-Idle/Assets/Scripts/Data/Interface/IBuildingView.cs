using Common.Pattern.BobbleEvent;
using Data.Model;
using UnityEngine;

namespace Data.Interface
{
    public interface IBuildingView : IMonoNode
    {
        GameObject GameObject { get; }

        void Init(BuildingModel model);
    }
}