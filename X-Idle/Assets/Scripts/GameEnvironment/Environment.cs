using Common;
using Common.Enum;
using Data;
using GameEnvironment.Controller;
using GameEnvironment.Factory;
using SceneLoader;
using UnityEngine.SceneManagement;


namespace GameEnvironment
{
    internal class Environment : IEnvironment
    {
        public Environment(IApplicationData data, IAppSceneLoader sceneLoader)
        {
            m_applicationData = data;
            m_sceneLoader = sceneLoader;
            m_factory = new EnvironmentFactory(m_applicationData.AssetLibrary);
            sceneLoader.SceneNotify.OnSceneLoaded += OnSceneLoadedHandler;
        }

        private readonly IApplicationData m_applicationData;
        private readonly IAppSceneLoader m_sceneLoader;
        private readonly IEnvironmentFactory m_factory;

        private void OnSceneLoadedHandler(Scene scene)
        {
            if (scene.name.Equals(nameof(SceneName.Game)))
            {
                var controller = scene.GetComponent<ISceneController>();
                controller.InitContent(m_factory);
            }
        }
    }
}