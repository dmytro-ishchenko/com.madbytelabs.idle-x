using System;

using GameEnvironment.Factory;

namespace GameEnvironment.Controller
{
    public interface ISceneController
    {
        event Action OnSceneInitialized;

        void InitContent(IEnvironmentFactory assetLibrary);
    }
}