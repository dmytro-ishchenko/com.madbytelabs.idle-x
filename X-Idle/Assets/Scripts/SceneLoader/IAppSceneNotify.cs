using System;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    public interface IAppSceneNotify
    {
        
        event Action<Scene> OnSceneLoaded;
        event Action<Scene> OnSceneUnloaded;
    }
}