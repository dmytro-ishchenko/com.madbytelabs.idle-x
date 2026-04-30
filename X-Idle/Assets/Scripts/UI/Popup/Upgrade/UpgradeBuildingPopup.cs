using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data.Enum;
using Data.Events;
using Data.Model.Popup;
using TMPro;
using UI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Upgrade
{
    internal class UpgradeBuildingPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private SerializedDictionary<RequireElementType, RequiredElementView> m_requiredElements;
        [SerializeField] private Transform m_root;
        [SerializeField] private Button m_upgrade;
        [SerializeField] private Button m_destroy;

        private readonly List<RequiredElementView> m_requirementViews = new();

        public override void Show<T>(T context)
        {
            if (context is UpgradeBuildingContext model)
            {
                m_name.text = model.Name;
                m_level.text = model.Level.ToString();
                m_icon.sprite = model.Icon;
                int elementIndex = 1;

                foreach (var element in model.Buildings)
                {
                    var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_root);
                    requirementElement.Init(element.Level >= element.RequireLevel, $"{element.Name} Level: {element.RequireLevel}");

                    requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                    rectTransform.SetSiblingIndex(elementIndex);

                    m_requirementViews.Add(requirementElement);

                    elementIndex += 1;
                }


                foreach (var element in model.Resources)
                {
                    var requirementElement = Instantiate(m_requiredElements[RequireElementType.Resource], m_root);

                    requirementElement.Init(element.Count >= element.RequireAmount, $"{element.Name} : {UiUtility.ValueToString(element.RequireAmount)}");

                    requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                    rectTransform.SetSiblingIndex(elementIndex);

                    m_requirementViews.Add(requirementElement);

                    elementIndex += 1;
                }

                if (!model.CanUpgrade)
                    m_upgrade.interactable = false;
                else
                {
                    m_upgrade.interactable = true;
                    m_upgrade.onClick.AddListener(() =>
                    {
                        Node.TriggerEvent(new BuildingProcessEventArgs(model.Id, model.TemplateId, BuildingActionType.UpgradeBuildingRequest));
                        Close();
                    });
                }

                m_destroy.onClick.AddListener(() =>
                {
                    Node.TriggerEvent(new BuildingProcessEventArgs(model.Id, model.TemplateId, BuildingActionType.DeleteBuildingRequest));
                    Close();
                });

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
            m_upgrade.onClick.RemoveAllListeners();
            m_destroy.onClick.RemoveAllListeners();

            base.Close();
        }
    }
}