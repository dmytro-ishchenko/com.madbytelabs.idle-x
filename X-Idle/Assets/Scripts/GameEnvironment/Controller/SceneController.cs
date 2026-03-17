using System;
using UnityEngine;

namespace GameEnvironment.Controller
{
    internal class SceneController : MonoBehaviour, ISceneController
    {
        public event Action OnSceneInitialized;
    }
}