using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data.Enum;
using Data.Model;
using UI.Model;
using UnityEngine;

namespace UI.Controller
{
    public class PlaceholderViewController : MonoNode
    {
        [SerializeField] private PlaceHolderViewElement m_pointUnlocked;
        [SerializeField] private PlaceHolderViewElement m_pointLocked;
        [SerializeField] private PlaceHolderViewElement m_pointBlocked;
        [SerializeField] private Transform m_root;
        private Camera m_camera;
        private bool m_locked = true;

        private Dictionary<string, PlaceHolderViewModel> m_placeholderData = new();
        private Dictionary<string, Transform> m_placeholderTransforms = new();

        public void Pause()
        {
            m_locked = true;
        }

        public void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData)
        {
            if (m_camera == null)
                m_camera = Camera.main;

            if (m_placeholderData.Count > 0)
            {
                m_locked = true;
                foreach (var element in m_placeholderData)
                {
                    Destroy(element.Value.View.gameObject);
                }

                m_placeholderData.Clear();
                m_placeholderTransforms.Clear();
            }


            foreach (var data in userPlaceHolderData)
            {
                m_placeholderTransforms.Add(data.Key, data.Value.Transform);

                PlaceHolderViewElement view = null;

                switch (data.Value.Status)
                {
                    case PlaceHolderStatus.Blocked:
                        view = Instantiate(m_pointBlocked, m_pointUnlocked.transform);
                        break;
                    case PlaceHolderStatus.Locked:
                        view = Instantiate(m_pointLocked, m_pointUnlocked.transform);
                        break;
                    case PlaceHolderStatus.Unlocked:
                        view = Instantiate(m_pointUnlocked, m_pointUnlocked.transform);
                        break;
                }

                if (view != null)
                {
                    view.transform.SetParent(m_root);
                    view.transform.localScale = Vector3.one;
                    m_placeholderData.Add(data.Key, new PlaceHolderViewModel(data.Key, data.Value.Status, view, data.Value.Transform));
                    view.Init(data.Key, data.Value.Status);
                    Node.AddChild(view.Node);

                    view.gameObject.SetActive(true);
                }
            }

            m_locked = false;
        }

        private void LateUpdate()
        {
            if (m_locked)
                return;
            foreach (var data in m_placeholderData)
            {
                if (m_locked)
                    break;

                Vector3 viewportPosition = m_camera.WorldToViewportPoint(data.Value.TargetTransform.position);

                bool isVisible =
                    viewportPosition is { z: > 0f, x: >= 0f and <= 1f, y: >= 0f and <= 1f };

                if (!isVisible)
                {
                    if (data.Value.View.gameObject.activeSelf)
                        data.Value.View.gameObject.SetActive(false);

                    continue;
                }

                if (!data.Value.View.gameObject.activeSelf)
                    data.Value.View.gameObject.SetActive(true);

                data.Value.View.transform.position = m_camera.WorldToScreenPoint(data.Value.TargetTransform.position);
            }
        }

        public void UpdatePlaceHoldersView(PlaceholderModel model)
        {
            m_locked = true;

            if (m_placeholderData.TryGetValue(model.Id, out PlaceHolderViewModel placeHolder))
            {
                switch (model.Status)
                {
                    case PlaceHolderStatus.Occupied:
                        Node.RemoveChild(placeHolder.View.Node);
                        Destroy(placeHolder.View.gameObject);
                        m_placeholderData.Remove(model.Id);
                        break;
                    case PlaceHolderStatus.Unlocked:
                        Node.RemoveChild(placeHolder.View.Node);
                        Destroy(placeHolder.View.gameObject);
                        ChangePlaceHolderView(m_pointUnlocked, placeHolder, model.Status);
                        break;
                    case PlaceHolderStatus.Blocked:
                        Node.RemoveChild(placeHolder.View.Node);
                        Destroy(placeHolder.View.gameObject);
                        ChangePlaceHolderView(m_pointBlocked, placeHolder, model.Status);
                        break;
                    case PlaceHolderStatus.Locked:
                        Node.RemoveChild(placeHolder.View.Node);
                        Destroy(placeHolder.View.gameObject);
                        ChangePlaceHolderView(m_pointLocked, placeHolder, model.Status);
                        break;
                }
            }
            else
            {
                AddPlaceHolderView(model.Status, model.Id);
            }

            m_locked = false;
        }

        void AddPlaceHolderView(PlaceHolderStatus status, string id)
        {
            PlaceHolderViewElement view = null;

            switch (status)
            {
                case PlaceHolderStatus.Blocked:
                    view = Instantiate(m_pointBlocked, m_pointUnlocked.transform);
                    break;
                case PlaceHolderStatus.Locked:
                    view = Instantiate(m_pointLocked, m_pointUnlocked.transform);
                    break;
                case PlaceHolderStatus.Unlocked:
                    view = Instantiate(m_pointUnlocked, m_pointUnlocked.transform);
                    break;
            }

            if (view != null)
            {
                view.transform.SetParent(m_root);
                view.transform.localScale = Vector3.one;
                m_placeholderData.Add(id, new PlaceHolderViewModel(id, status, view, m_placeholderTransforms[id]));
                view.Init(id, status);
                Node.AddChild(view.Node);

                view.gameObject.SetActive(true);
            }
        }

        void ChangePlaceHolderView(PlaceHolderViewElement source, PlaceHolderViewModel model, PlaceHolderStatus newStatus)
        {
            var view = Instantiate(source, m_pointUnlocked.transform);
            view.transform.SetParent(m_root);
            view.transform.localScale = Vector3.one;
            m_placeholderData[model.Id] = new PlaceHolderViewModel(model.Id, newStatus, view, m_placeholderData[model.Id].TargetTransform);
            view.Init(model.Id, newStatus);
            Node.AddChild(view.Node);

            view.gameObject.SetActive(true);
        }
    }
}