using System;
using System.Collections.Generic;
using System.Linq;
using Common.Pattern.BobbleEvent;
using Data.Model;
using GameEnvironment.Factory;
using UnityEngine;

namespace GameEnvironment.Controller
{
    internal class SceneController : MonoBehaviour, ISceneController
    {
        [SerializeField] private PlaceholderRoot m_placeholderRoot;
        private IEnvironmentFactory m_factory;
        public event Action OnSceneInitialized;
        public Node Node { get; private set; }

        public void InitContent(IList<BuildingModel> userBuildings, IEnvironmentFactory factory)
        {
            m_factory = factory;
            Node = new();

            foreach (var placeHolder in m_placeholderRoot.PlaceHolders)
            {
                var building = userBuildings.FirstOrDefault(b => b.Id == placeHolder.Id);

                if (building == null)
                    continue;

                var view = m_factory.GetView(building.Id, building.Template);

                placeHolder.AddContent(view);
                Node.AddChild(view.Node);
            }

            OnSceneInitialized?.Invoke();
        }

        public void CreateBuilding(BuildingModel model, IEnvironmentFactory factory)
        {
            var placeHolder = m_placeholderRoot.PlaceHolders.FirstOrDefault(b => b.Id == model.Id);

            if (placeHolder == null)
                return;

            var view = m_factory.GetView(model.Id, model.Template);

            Node.RemoveChild(placeHolder.View.Node);
            placeHolder.RemoveContent();
            placeHolder.AddContent(view);

            Node.AddChild(view.Node);
        }

        public void DeleteBuilding(BuildingModel model, IEnvironmentFactory factory)
        {
            var placeHolder = m_placeholderRoot.PlaceHolders.FirstOrDefault(b => b.Id == model.Id);

            if (placeHolder == null)
                return;

            var view = m_factory.GetView(model.Id, model.Template);

            Node.RemoveChild(placeHolder.View.Node);
            placeHolder.RemoveContent();
            placeHolder.AddContent(view);

            Node.AddChild(view.Node);
        }

        public Transform GetPlaceholderTransform(string id)
        {
            foreach (var element in m_placeholderRoot.PlaceHolders)
            {
                if (element.Id.Equals(id))
                {
                    return element.transform;
                }
            }

            return null;
        }
    }
}