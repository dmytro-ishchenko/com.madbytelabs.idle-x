using System;
using Common;
using Common.Enum;
using Common.Events;
using Common.Pattern.BobbleEvent;
using Data;
using GameEnvironment.Controller;
using GameEnvironment.Factory;
using SceneLoader;
using UnityEngine.SceneManagement;


namespace GameEnvironment
{
    internal class Environment : IEnvironment, IRootEventHandler
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
        public event Action<SelectContentEventArgs> OnSelectContent;

        private void OnSceneLoadedHandler(Scene scene)
        {
            if (scene.name.Equals(nameof(SceneName.Game)))
            {
                var controller = scene.GetComponent<ISceneController>();
                controller.InitContent(m_factory);
                controller.Node.SetRootHandler(this);
            }
        }

        public void Handle(NodeEvent evt)
        {
            switch (evt.EventName)
            {
                case "SelectContent":
                    OnSelectContent?.Invoke((SelectContentEventArgs)evt.EventArgs);
                    break;
            }
        }
    }
}