using Common.Pattern.BobbleEvent;
using Data.Enum;
using Data.Events;
using Data.Interface;
using Data.Model;
using UnityEngine;

namespace GameEnvironment.View
{
    internal class BuildingView : MonoBehaviour, IBuildingView
    {
        private float m_threshold = 0.1f;
        private Vector2 m_touchPosition;
        public GameObject GameObject => gameObject;
        public BuildingModel Model { get; private set; }

        public void Init(BuildingModel model)
        {
            Model = model;
            Node = new();
        }

        private void OnMouseDown()
        {
            m_touchPosition = Input.mousePosition;
        }

        private void OnMouseUp()
        {
            if (Vector3.Distance(m_touchPosition, Input.mousePosition) > m_threshold)
                return;

            if (Model.Level == 0)
                Node.TriggerEvent("BuildingAction", new BuildingEventArgs(Model, BuildingActionType.CreateBuilding));
            else
                Node.TriggerEvent("BuildingAction", new BuildingEventArgs(Model, BuildingActionType.UpgradeBuilding));
        }

        public Node Node { get; private set; }
    }
}