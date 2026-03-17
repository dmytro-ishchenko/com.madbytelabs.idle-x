using System;
using Common.Enum;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    internal interface IAppSceneManager : IAppSceneNotify
    {
        void LoadScene(SceneName sceneName, LoadSceneMode mode);
        void UnloadScene(SceneName sceneName);
    }
}