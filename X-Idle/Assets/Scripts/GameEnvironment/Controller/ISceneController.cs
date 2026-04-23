using System;
using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data.Model;
using GameEnvironment.Factory;

namespace GameEnvironment.Controller
{
    public interface ISceneController: IMonoNode
    {
        event Action OnSceneInitialized;

        void InitContent(IList<BuildingModel> userBuildings, IEnvironmentFactory factory);
        void CreateBuilding(BuildingModel model, IEnvironmentFactory factory);
    }
}