using System;

namespace GameEnvironment.Controller
{
    public interface ISceneController
    {
        event Action OnSceneInitialized;
    }
}