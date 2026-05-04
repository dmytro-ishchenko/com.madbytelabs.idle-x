using System.Collections.Generic;
using Data.Enum;
using Data.Events;
using Data.Model.Popup;
using UI.Popup.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Create
{
    internal class CreateBuildingPopup : BasePopup
    {
        [SerializeField] private BuildingElement m_source;
        [SerializeField] private Transform m_root;
        [SerializeField] private ScrollRect m_scrollRect;
        [SerializeField] Button m_createButton;
        private List<BuildingElement> m_elements = new();
        private string m_selectedTemplateId;
        private string m_selectedPlaceHolderId;

        public override void Show<T>(T context)
        {
            m_scrollRect.verticalNormalizedPosition = 1.0f;
            if (context is CreateBuildingContext model)
            {
                foreach (var buildingTemplate in model.Buildings)
                {
                    var element = Instantiate(m_source);
                    element.transform.SetParent(m_root);
                    element.Init(buildingTemplate);
                    element.gameObject.SetActive(true);
                    m_elements.Add(element);
                    element.OnSelect += OnSelectHandler;
                }

                m_selectedPlaceHolderId = model.PlaceholderId;
                base.Show(context);
            }

            m_createButton.interactable = false;
            m_selectedTemplateId = string.Empty;

            m_createButton.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(m_selectedTemplateId))
                {
                    Node.TriggerEvent(new BuildingProcessEventArgs(m_selectedPlaceHolderId, m_selectedTemplateId, BuildingActionType.CreateBuildingRequest));
                }

                Close();
            });
        }

        public override void Close()
        {
            foreach (var element in m_elements)
            {
                element.OnSelect -= OnSelectHandler;
                Destroy(element.gameObject);
            }

            m_elements.Clear();

            m_createButton.onClick.RemoveAllListeners();

            base.Close();
        }

        private void OnSelectHandler(BuildingElement element)
        {
            foreach (var el in m_elements)
            {
                el.Select(false);
            }

            element.Select(true);

            if (!m_createButton.interactable)
                m_createButton.interactable = true;
            m_selectedTemplateId = element.Id;
        }
    }
}