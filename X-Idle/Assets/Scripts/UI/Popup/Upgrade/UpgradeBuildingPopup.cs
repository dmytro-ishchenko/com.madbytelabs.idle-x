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
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private TMP_Text m_nextLevel;
        [SerializeField] private Transform m_levelRoot;
        [SerializeField] private Transform m_nextLevelRoot;
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
                m_name.text = model.Name;
                m_level.text = $"Lv.{model.Level}";
                m_nextLevel.text = $"Lv.{model.Level + 1}";
                m_icon.sprite = model.Icon;


                foreach (var currentModel in model.CurrentInfoModels)
                {
                    var infoView = Instantiate(m_infoElement, m_levelRoot);
                    infoView.Init(currentModel.Icon, currentModel.Text, currentModel.Value);
                    m_infoElements.Add(infoView);
                    infoView.gameObject.SetActive(true);
                }

                foreach (var nextModel in model.NextInfoModels)
                {
                    var infoView = Instantiate(m_infoElement, m_nextLevelRoot);
                    infoView.Init(nextModel.Icon, nextModel.Text, nextModel.Value);
                    m_infoElements.Add(infoView);
                    infoView.gameObject.SetActive(true);
                }

             //   m_leverBlock.sizeDelta = new Vector2(m_leverBlock.sizeDelta.x, m_levelElement.sizeDelta.y + 40);

                foreach (var element in model.Buildings)
                {
                    var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_requirmentRoot);
                    requirementElement.Init(element.Level >= element.RequireLevel, $"{element.Name} Level: {element.RequireLevel}");
                    m_requirementViews.Add(requirementElement);
                    requirementElement.gameObject.SetActive(true);
                }


                foreach (var element in model.Resources)
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
                    m_upgrade.interactable = true;
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
    }
}