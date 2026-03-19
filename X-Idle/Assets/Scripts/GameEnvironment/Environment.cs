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
    internal class Environment : IEnvironment, IRootEventHandler
    {
        public Environment(IApplicationData data, IAppSceneLoader sceneLoader)
        {
            m_applicationData = data;
            m_factory = new EnvironmentFactory(m_applicationData.AssetLibrary);
            sceneLoader.SceneNotify.OnSceneLoaded += OnSceneLoadedHandler;
        }

        private readonly IApplicationData m_applicationData;
        private readonly IEnvironmentFactory m_factory;
        public event Action<BuildingEventArgs> OnCreateBuildingRequest;
        public event Action<BuildingEventArgs> OnUpgradeBuildingRequest;

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
                case "BuildingAction":
                    if (evt.EventArgs is BuildingEventArgs args)
                    {
                        switch (args.BuildingActionType)
                        {
                            case BuildingActionType.CreateBuilding:
                                OnCreateBuildingRequest?.Invoke((BuildingEventArgs)evt.EventArgs);
                                break;
                            case BuildingActionType.UpgradeBuilding:
                                OnUpgradeBuildingRequest?.Invoke((BuildingEventArgs)evt.EventArgs);
                                break;
                        }
                    }


                    break;
            }
        }
    }
}