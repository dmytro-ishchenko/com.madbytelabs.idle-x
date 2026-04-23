using System;
using Common;
using Common.Enum;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Enum;
using Data.Events;
using GameEnvironment.Controller;
using GameEnvironment.Factory;
using SceneLoader;
using UnityEngine.SceneManagement;


namespace GameEnvironment
{
    internal class Environment : BobbleDispatcher, IEnvironment
    {
        public Environment(IApplicationData data, IAppSceneLoader sceneLoader)
        {
            m_applicationData = data;
            m_factory = new EnvironmentFactory(m_applicationData.AssetLibrary);
            sceneLoader.SceneNotify.OnSceneLoaded += OnSceneLoadedHandler;

            Subscribe<BuildingRequestEventArgs>(BuildingActionHandler);
        }

        private readonly IApplicationData m_applicationData;
        private readonly IEnvironmentFactory m_factory;
        public event Action<BuildingRequestEventArgs> OnBuildingActionRequest;


        private void OnSceneLoadedHandler(Scene scene)
        {
            if (scene.name.Equals(nameof(SceneName.Game)))
            {
                var controller = scene.GetComponent<ISceneController>();
                
                controller.InitContent(m_applicationData.UserBuildings, m_factory);
                controller.Node.SetDispatcher(this);
            }
        }

        private void BuildingActionHandler(BuildingRequestEventArgs args)
        {
            OnBuildingActionRequest?.Invoke(args);
        }
    }
}