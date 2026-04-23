using Common.Enum;
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
            IAppSceneLoader sceneLoader = new AppSceneLoader();
            IApplicationData data = new ApplicationData();
            IGameplay gameplay = new Gameplay(data, sceneLoader);
            IEnvironment environment = new Environment(data, sceneLoader);
            IEntitySystem entitySystem = new EntitySystem();
            IUISystem uiSystem = new UISystem(data, environment, sceneLoader);

            data.InitApplicationData(() => { sceneLoader.SwitchToScene(SceneName.Game); });
        }
    }
}