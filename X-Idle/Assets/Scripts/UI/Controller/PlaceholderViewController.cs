using System;
using System.Collections.Generic;
using Data.Enum;
using Data.Model;
using UnityEngine;

namespace UI.Controller
{
    public class PlaceholderViewController : MonoBehaviour
    {
        [SerializeField] private GameObject m_pointUnlocked;
        [SerializeField] private GameObject m_pointLocked;
        [SerializeField] private GameObject m_pointBlocked;
        [SerializeField] private Transform m_root;
        [SerializeField] private Camera m_camera;

        private Dictionary<string, PlaceHolderViewModel> m_placeholderData = new();

        public void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData)
        {
            foreach (var data in userPlaceHolderData)
            {
                GameObject view = null;

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
                    m_placeholderData.Add(data.Key, new PlaceHolderViewModel(view, data.Value.Transform));
                    view.SetActive(true);
                }
            }
        }

        private void Update()
        {
            foreach (var data in m_placeholderData)
            {
                Vector3 screenPos = m_camera.WorldToScreenPoint(data.Value.TargetTransform.position);

                data.Value.View.transform.position = screenPos;
            }
        }

        class PlaceHolderViewModel
        {
            public PlaceHolderViewModel(GameObject view, Transform targetTransform)
            {
                View = view;
                TargetTransform = targetTransform;
            }

            public GameObject View { get; }
            public Transform TargetTransform { get; }
        }
    }
}