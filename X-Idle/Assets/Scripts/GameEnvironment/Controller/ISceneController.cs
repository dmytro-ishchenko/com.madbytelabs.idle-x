using System;
using Common.Pattern.BobbleEvent;
using GameEnvironment.Factory;

namespace GameEnvironment.Controller
{
    public interface ISceneController: IMonoNode
    {
        event Action OnSceneInitialized;

        void InitContent(IEnvironmentFactory assetLibrary);
    }
}