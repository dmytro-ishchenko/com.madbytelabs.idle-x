using System;
using System.Collections.Generic;
using Data;
using Data.Enum;
using Data.Events;
using Data.Interface;
using Data.Model.Popup;
using UI.Popup.Controller;
using UI.Popup.Upgrade;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Popup.Create
{
    internal class CreateBuildingPopup : BasePopup
    {
        [SerializeField] private BuildingElement m_source;
        [SerializeField] private Transform m_root;
        [SerializeField] private ScrollRect m_scrollRect;
        [SerializeField] private BuildingInfo m_info;
        [SerializeField] private RequiredModel m_requiredBuildings;
        [SerializeField] private RequiredModel m_requiredResources;
        [SerializeField] Button m_createButton;
        [SerializeField] private GameObject m_canNotBuilding;
        private List<BuildingElement> m_elements = new();
        private string m_selectedTemplateId;
        private string m_selectedPlaceHolderId;
        private IApplicationData m_applicationData;
        private List<RequiredElementView> m_requiredElements = new();
        private float m_scrollHeight;

        public override void Show<T>(T context)
        {
            m_scrollRect.verticalNormalizedPosition = 1.0f;
            m_scrollHeight = m_scrollRect.content.sizeDelta.y;
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
                m_applicationData = model.ApplicationData;
                base.Show(context);
            }

            m_selectedTemplateId = string.Empty;

            m_createButton.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(m_selectedTemplateId))
                {
                    Node.TriggerEvent(new BuildingProcessEventArgs(m_selectedPlaceHolderId, m_selectedTemplateId, BuildingActionType.CreateBuildingRequest));
                }

                Close();
            });

            m_info.Block.SetActive(false);
            m_requiredBuildings.Block.SetActive(false);
            m_requiredResources.Block.SetActive(false);
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
            m_selectedTemplateId = element.Template.Id;
            InitBuildingInfo(element.Template);
        }

        void InitBuildingInfo(IBuildingTemplate template)
        {
            m_info.BuildingName.text = template.Name;
            m_info.BuildingDescription.text = template.ShortDescription;

            var createContext = m_applicationData.GetCreateBuildingContext(template);

            foreach (var element in m_requiredElements)
            {
                Destroy(element.gameObject);
            }

            m_requiredElements.Clear();

            bool canCreate = true;

            if (createContext.Buildings is { Count: > 0 })
            {
                m_requiredBuildings.Block.SetActive(true);

                foreach (var element in createContext.Buildings)
                {
                    var instance = Instantiate(m_requiredBuildings.Source);
                    instance.transform.SetParent(m_requiredBuildings.Root);
                    instance.Init(element.Level >= element.RequireLevel, null, $"{element.Name} Lv.{element.RequireLevel}");
                    instance.gameObject.SetActive(true);
                    m_requiredElements.Add(instance);

                    if (canCreate)
                    {
                        canCreate = element.Level >= element.RequireLevel;
                    }
                }
            }
            else
            {
                m_requiredBuildings.Block.SetActive(false);
            }

            if (createContext.Resources is { Count: > 0 })
            {
                m_requiredResources.Block.SetActive(true);
                foreach (var element in createContext.Resources)
                {
                    var instance = Instantiate(m_requiredResources.Source);
                    instance.transform.SetParent(m_requiredResources.Root);
                    instance.Init(element.Count >= element.RequireAmount, null, $"{element.Name} Lv.{element.RequireAmount}");
                    instance.gameObject.SetActive(true);
                    m_requiredElements.Add(instance);
                    if (canCreate)
                    {
                        canCreate = element.Count >= element.RequireAmount;
                    }
                }
            }
            else
            {
                m_requiredResources.Block.SetActive(false);
            }

            m_info.Block.SetActive(true);

            if (canCreate)
            {
                m_createButton.gameObject.SetActive(true);
                m_canNotBuilding.SetActive(false);
            }
            else
            {
                m_createButton.gameObject.SetActive(false);
                m_canNotBuilding.SetActive(true);
            }
        }

        [Serializable]
        class BuildingInfo
        {
            public GameObject Block;
            public TMP_Text BuildingName;
            public TMP_Text BuildingDescription;
        }

        [Serializable]
        class RequiredModel
        {
            public GameObject Block;
            public Transform Root;
            public RequiredElementView Source;
        }
    }
}