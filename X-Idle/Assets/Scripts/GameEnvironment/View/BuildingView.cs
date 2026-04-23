using Common.Pattern.BobbleEvent;
using Data.Enum;
using Data.Events;
using Data.Interface;
using Data.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameEnvironment.View
{
    internal class BuildingView : MonoBehaviour, IBuildingView, IPointerDownHandler, IPointerUpHandler
    {
        private float m_threshold = 0.1f;
        private Vector2 m_touchPosition;
        public GameObject GameObject => gameObject;
        public BuildingModel Model { get; private set; }

        public Node Node { get; } = new();

        public void Init(BuildingModel model)
        {
            Model = model;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_EDITOR
            m_touchPosition = Mouse.current.position.ReadValue();
#else
            m_touchPosition = Touchscreen.current.position.ReadValue();
#endif
        }

        public void OnPointerUp(PointerEventData eventData)
        {
#if UNITY_EDITOR

            if (Vector3.Distance(m_touchPosition, Mouse.current.position.ReadValue()) > m_threshold)
                return;
#else
            if (Vector3.Distance(m_touchPosition, Touchscreen.current.position.ReadValue()) > m_threshold)
                return;
#endif
            if (Model.Template.BuildingContext.BuildingType == BuildingType.DestroyedBuilding)
                Node.TriggerEvent(new BuildingRequestEventArgs(Model, BuildingActionType.CreateBuildingRequest));
            else
                Node.TriggerEvent(new BuildingRequestEventArgs(Model, BuildingActionType.UpgradeBuildingRequest));
        }
    }
}