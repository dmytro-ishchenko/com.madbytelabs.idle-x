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
                view.GameObject.transform.SetParent(placeHolder.transform);
                view.GameObject.transform.localPosition = Vector3.zero;
                view.GameObject.transform.localEulerAngles = Vector3.zero;

                Node.AddChild(view.Node);
            }

            OnSceneInitialized?.Invoke();
        }

        public Node Node { get; private set; }
    }
}