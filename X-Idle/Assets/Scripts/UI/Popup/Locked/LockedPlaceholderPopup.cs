using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data.Enum;
using Data.Events;
using Data.Model.Popup;
using TMPro;
using UI.Popup.Upgrade;
using UI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Locked
{
    internal class LockedPlaceholderPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private SerializedDictionary<RequireElementType, RequiredElementView> m_requiredElements;
        [SerializeField] private Transform m_root;
        [SerializeField] private Button m_unlock;

        private readonly List<RequiredElementView> m_requirementViews = new();

        public override void Show<T>(T context)
        {
            if (context is LockedPlaceHolderContext model)
            {
                int elementIndex = 1;
                if (model.RequirementsModel.Buildings != null)
                {
                    foreach (var element in model.RequirementsModel.Buildings)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_root);
                        requirementElement.Init(element.Level >= element.RequireLevel, $"{element.Name} Level: {element.RequireLevel}");

                        requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                        rectTransform.SetSiblingIndex(elementIndex);

                        m_requirementViews.Add(requirementElement);

                        elementIndex += 1;
                    }
                }

                if (model.RequirementsModel.Resources != null)
                {
                    foreach (var element in model.RequirementsModel.Resources)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Resource], m_root);

                        requirementElement.Init(element.Count >= element.RequireAmount, $"{element.Name} : {UiUtility.ValueToString(element.RequireAmount)}");

                        requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                        rectTransform.SetSiblingIndex(elementIndex);

                        m_requirementViews.Add(requirementElement);

                        elementIndex += 1;
                    }
                }

                if (!model.RequirementsModel.CanUpgrade)
                    m_unlock.interactable = false;
                else
                {
                    m_unlock.interactable = true;
                    m_unlock.onClick.AddListener(() =>
                    {
                        Node.TriggerEvent(new UnlockPlaceholderEventArgs(model.Id));
                        Close();
                    });
                }


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
            m_unlock.onClick.RemoveAllListeners();

            base.Close();
        }
    }
}