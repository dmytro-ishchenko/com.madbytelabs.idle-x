using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data.Enum;
using Data.Events;
using Data.Model.Popup;
using Data.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Upgrade
{
    internal class UpgradeBuildingPopup : BasePopup
    {
        [SerializeField] private BuildingBlock m_buildingBlock;
        [SerializeField] private OutputBlock m_outputBlock;
        [SerializeField] private UseInfoModel m_useInfoModel;
        [SerializeField] private SerializedDictionary<RequireElementType, RequiredElementView> m_requiredElements;
        [SerializeField] private Transform m_requirmentRoot;
        [SerializeField] private Button m_upgrade;
        [SerializeField] private GameObject m_notMeat;
        private readonly List<RequiredElementView> m_requirementViews = new();
        private readonly List<UseInfoElement> m_infoElements = new();

        public override void Show<T>(T context)
        {
            if (context is UpgradeBuildingContext model)
            {
                m_buildingBlock.Name.text = model.Name;
                m_buildingBlock.Level.text = $"Lv.{model.Level}";
                m_buildingBlock.NextLevel.text = $"Lv.{model.Level + 1}";
                m_buildingBlock.Icon.sprite = model.Icon;

                m_outputBlock.BlockName.text = model.OutputModel.BlockName;
                m_outputBlock.Icon.sprite = model.OutputModel.Icon;
                m_outputBlock.ResourceName.text = model.OutputModel.ResourceName;

                if (model.OutputModel.ShowAsBonus)
                {
                    if (model.OutputModel.Resource != null)
                    {
                        m_outputBlock.ProductionRoot.SetActive(true);
                        m_outputBlock.StorageRoot.SetActive(false);
                        m_outputBlock.CurrentProductionField.text = model.OutputModel.Resource.CurrentOutput;
                        m_outputBlock.NextProductionField.text = model.OutputModel.Resource.NextOutput;
                    }
                    else
                    {
                        m_outputBlock.ProductionRoot.SetActive(false);
                        m_outputBlock.StorageRoot.SetActive(true);
                        m_outputBlock.StorageProgressRoot.SetActive(true);
                        m_outputBlock.StorageView.gameObject.SetActive(false);
                        m_outputBlock.CurrentStorageField.text = model.OutputModel.Storage.CurrentOutput;
                        m_outputBlock.NextStorageField.text = model.OutputModel.Storage.NextOutput;
                    }
                }
                else
                {
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

                        m_outputBlock.StorageProgressRoot.SetActive(false);
                        m_outputBlock.StorageView.gameObject.SetActive(true);

                        m_outputBlock.StorageView.text = model.OutputModel.Storage.CurrentOutput;
                    }
                    else
                    {
                        m_outputBlock.StorageRoot.SetActive(false);
                    }
                }


                if (model.UseResourcesModels is { Count: > 0 })
                {
                    m_useInfoModel.UseRoot.SetActive(true);
                    foreach (var useModel in model.UseResourcesModels)
                    {
                        var infoView = Instantiate(m_useInfoModel.UseInfoElement, m_useInfoModel.Root);
                        infoView.Init(useModel.Icon, useModel.ResourceName, useModel.CurrentUse, useModel.NextUse);
                        m_infoElements.Add(infoView);
                        infoView.gameObject.SetActive(true);
                    }
                }
                else
                {
                    m_useInfoModel.UseRoot.SetActive(false);
                }

                if (model.RequirementsModel.Buildings != null)
                    foreach (var element in model.RequirementsModel.Buildings)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building],
                            m_requirmentRoot);
                        requirementElement.Init(element.Level >= element.RequireLevel, null,
                            $"{element.Name} Level: {element.RequireLevel}");
                        m_requirementViews.Add(requirementElement);
                        requirementElement.gameObject.SetActive(true);
                    }

                if (model.RequirementsModel.Resources != null)
                    foreach (var element in model.RequirementsModel.Resources)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Resource],
                            m_requirmentRoot);
                        requirementElement.Init(element.Count >= element.RequireAmount, null,
                            $"{element.Name} : {DataUtility.ValueToString(element.RequireAmount)}");
                        m_requirementViews.Add(requirementElement);
                        requirementElement.gameObject.SetActive(true);
                    }

                if (!model.CanUpgrade)
                {
                    m_upgrade.gameObject.SetActive(false);
                    m_notMeat.SetActive(true);
                }
                else
                {
                    m_upgrade.gameObject.SetActive(true);
                    m_notMeat.SetActive(false);
                    m_upgrade.onClick.AddListener(() =>
                    {
                        Node.TriggerEvent(new BuildingProcessEventArgs(model.Id, model.TemplateId,
                            BuildingActionType.UpgradeBuildingRequest));
                        Close();
                    });
                }


                base.Show(context);

                resizer.Recalculate();
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
            public TMP_Text BlockName;
            public Image Icon;
            public TMP_Text ResourceName;
            public GameObject ProductionRoot;
            public TMP_Text CurrentProductionField;
            public TMP_Text NextProductionField;
            public GameObject StorageRoot;
            public TMP_Text CurrentStorageField;
            public TMP_Text NextStorageField;
            public GameObject StorageProgressRoot;
            public TMP_Text StorageView;
        }

        [Serializable]
        class UseInfoModel
        {
            public GameObject UseRoot;
            public UseInfoElement UseInfoElement;
            public Transform Root;
        }
    }
}