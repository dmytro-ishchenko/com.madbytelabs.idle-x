using Common.Pattern.BobbleEvent;
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
        private string m_id;
        public GameObject GameObject => gameObject;


        public Node Node { get; } = new();

        public void Init(BuildingModel model)
        {
            m_id = model.Id;
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

            Node.TriggerEvent(new BuildingRequestEventArgs(m_id));
        }
    }
}