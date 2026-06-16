using System;
using Common;
using Common.Enum;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Enum;
using Data.Events;
using Data.Model;
using GameEnvironment.Controller;
using GameEnvironment.Factory;
using SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GameEnvironment
{
    internal class Environment : BobbleDispatcher, IEnvironment
    {
        public Environment(IApplicationData data, IAppSceneLoader sceneLoader)
        {
            m_applicationData = data;
            m_factory = new EnvironmentFactory();
            m_sceneLoader = sceneLoader;
            m_sceneLoader.SceneNotify.OnSceneLoaded += OnSceneLoadedHandler;

            m_applicationData.OnBuildingCreated += OnBuildingCreatedHandler;
            m_applicationData.OnBuildingDeleted += OnBuildingDeletedHandler;
            m_applicationData.OnAppStatusChanged += OnAppStatusChangedHandler;
            Subscribe<BuildingRequestEventArgs>(BuildingActionHandler);
        }

        private readonly IApplicationData m_applicationData;
        private readonly IEnvironmentFactory m_factory;
        private ISceneController m_sceneController;
        private IAppSceneLoader m_sceneLoader;
        public event Action<BuildingRequestEventArgs> OnBuildingActionRequest;

        public Transform GetPlaceholderTransform(string id) => m_sceneController.GetPlaceholderTransform(id);

        private void OnSceneLoadedHandler(Scene scene)
        {
            if (scene.name.Equals(nameof(SceneName.Game)))
            {
                m_sceneController = scene.GetComponent<ISceneController>();
                m_applicationData.OnProgressReset += InitController;

                InitController();
            }
        }


        private void InitController()
        {
            m_sceneController.InitContent(m_applicationData.UserBuildings, m_factory);
            m_sceneController.Node.SetDispatcher(this);
        }

        private void BuildingActionHandler(BuildingRequestEventArgs args)
        {
            OnBuildingActionRequest?.Invoke(args);
        }

        private void OnBuildingCreatedHandler(BuildingModel model)
        {
            m_sceneController.CreateBuilding(model, m_factory);
        }

        private void OnBuildingDeletedHandler(BuildingModel model)
        {
            m_sceneController.DeleteBuilding(model, m_factory);
        }

        private void OnAppStatusChangedHandler(AppStatus status)
        {
            if (status == AppStatus.Pause)
            {
                m_applicationData.OnProgressReset -= InitController;
                m_sceneLoader.UnloadScene(SceneName.Game);
            }else if (status == AppStatus.Resuming)
            {
                m_sceneLoader.AddScene(SceneName.Game);
            }
        }
    }
}