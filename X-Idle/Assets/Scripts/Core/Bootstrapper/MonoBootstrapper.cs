using Data;
using GameEntity;
using GameEnvironment;
using GameLoop;
using SceneLoader;
using UI;
using UnityEngine;

namespace Core.Bootstrapper
{
    public class MonoBootstrapper : MonoBehaviour, IBootstrapper
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            var app = new Application(this);
        }


        public void Bootstrap()
        {
            IAppSceneSwitcher sceneSwitcher = new  AppSceneSwitcher();
            IApplicationData data = new ApplicationData();
            IEnvironment environment = new Environment();
            IGameplay gameplay = new Gameplay();
            IEntitySystem entitySystem = new EntitySystem();
            IGameUI gameUI = new GameUI();
        }
    }
}