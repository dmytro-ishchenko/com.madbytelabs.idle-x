using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data.Enum;
using Data.Events;
using Data.Model.Popup;
using Data.Utility;
using TMPro;
using UI.Popup.Info;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Upgrade
{
    internal class UpgradeBuildingPopup : BasePopup
    {
        [SerializeField] private BuildingBlock m_buildingBlock;
        [SerializeField] private OutputBlock m_outputBlock;

        [SerializeField] private InfoElement m_infoElement;
        [SerializeField] private SerializedDictionary<RequireElementType, RequiredElementView> m_requiredElements;
        [SerializeField] private Transform m_requirmentRoot;
        [SerializeField] private Button m_upgrade;
        [SerializeField] private Button m_close;
        [SerializeField] private RectTransform m_leverBlock;
        [SerializeField] private RectTransform m_levelElement;
        private readonly List<RequiredElementView> m_requirementViews = new();
        private readonly List<InfoElement> m_infoElements = new();

        public override void Show<T>(T context)
        {
            if (context is UpgradeBuildingContext model)
            {
                m_buildingBlock.Name.text = model.Name;
                m_buildingBlock.Level.text = $"Lv.{model.Level}";
                m_buildingBlock.NextLevel.text = $"Lv.{model.Level + 1}";
                m_buildingBlock.Icon.sprite = model.Icon;


                m_outputBlock.Icon.sprite = model.OutputModel.Icon;
                m_outputBlock.ResourceName.text = model.OutputModel.ResourceName;

                if (model.OutputModel.Resource != null)
                {
                    m_outputBlock.ProductionRoot.SetActive(true);
                    m_outputBlock.CurrentProductionField.text = model.OutputModel.Resource.CurrentOutput;
                    m_outputBlock.NextProductionField.text = model.OutputModel.Resource.NextOutput;
                }
                else
                {
                    m_outputBlock.ProductionRoot.SetActive(false);
                }

                if (model.OutputModel.Storage != null)
                {
                    m_outputBlock.StorageRoot.SetActive(true);
                    m_outputBlock.CurrentStorageField.text = model.OutputModel.Storage.CurrentOutput;
                    m_outputBlock.NextStorageField.text = model.OutputModel.Storage.NextOutput;
                }
                else
                {
                    m_outputBlock.StorageRoot.SetActive(false);
                }

                // m_outputBlock.ResourceName=model.
                // m_outputBlock.CurrentProductionField.text = model.OutputModel.CurrentOutput;
                // m_outputBlock.NextProductionField.text = model.OutputModel.NextOutput;


                // foreach (var currentModel in model.CurrentInfoModels)
                // {
                //     var infoView = Instantiate(m_infoElement, m_levelRoot);
                //     infoView.Init(currentModel.Icon, currentModel.Text, currentModel.Value);
                //     m_infoElements.Add(infoView);
                //     infoView.gameObject.SetActive(true);
                // }
                //
                // foreach (var nextModel in model.NextInfoModels)
                // {
                //     var infoView = Instantiate(m_infoElement, m_nextLevelRoot);
                //     infoView.Init(nextModel.Icon, nextModel.Text, nextModel.Value);
                //     m_infoElements.Add(infoView);
                //     infoView.gameObject.SetActive(true);
                // }

                if (model.RequirementsModel.Buildings != null)
                    foreach (var element in model.RequirementsModel.Buildings)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_requirmentRoot);
                        requirementElement.Init(element.Level >= element.RequireLevel, $"{element.Name} Level: {element.RequireLevel}");
                        m_requirementViews.Add(requirementElement);
                        requirementElement.gameObject.SetActive(true);
                    }

                if (model.RequirementsModel.Resources != null)
                    foreach (var element in model.RequirementsModel.Resources)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Resource], m_requirmentRoot);
                        requirementElement.Init(element.Count >= element.RequireAmount, $"{element.Name} : {DataUtility.ValueToString(element.RequireAmount)}");
                        m_requirementViews.Add(requirementElement);
                        requirementElement.gameObject.SetActive(true);
                    }

                if (!model.CanUpgrade)
                {
                    m_upgrade.gameObject.SetActive(false);
                    m_close.gameObject.SetActive(true);
                }
                else
                {
                    m_upgrade.gameObject.SetActive(true);
                    m_close.gameObject.SetActive(false);
                    m_upgrade.onClick.AddListener(() =>
                    {
                        Node.TriggerEvent(new BuildingProcessEventArgs(model.Id, model.TemplateId, BuildingActionType.UpgradeBuildingRequest));
                        Close();
                    });
                }

                m_close.onClick.AddListener(Close);

                base.Show(context);
            }
        }

        public override void Close()
        {
            foreach (var requirementView in m_requirementViews)
            {
                Destroy(requirementView.gameObject);
            }

            m_requirementViews.Clear();

            foreach (var infoElement in m_infoElements)
            {
                Destroy(infoElement.gameObject);
            }

            m_infoElements.Clear();

            m_upgrade.onClick.RemoveAllListeners();
            m_close.onClick.RemoveAllListeners();

            base.Close();
        }

        [Serializable]
        class BuildingBlock
        {
            public TMP_Text Name;
            public Image Icon;
            public TMP_Text Level;
            public TMP_Text NextLevel;
        }

        [Serializable]
        class OutputBlock
        {
            public Image Icon;
            public TMP_Text ResourceName;
            public GameObject ProductionRoot;
            public TMP_Text CurrentProductionField;
            public TMP_Text NextProductionField;
            public GameObject StorageRoot;
            public TMP_Text CurrentStorageField;
            public TMP_Text NextStorageField;
        }
    }
}