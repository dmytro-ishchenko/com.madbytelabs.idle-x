using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data.Events;
using Data.Model.Popup;
using Data.Utility;
using TMPro;
using UI.Popup.Upgrade;
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
        [SerializeField] private GameObject m_notMet;

        private readonly List<RequiredElementView> m_requirementViews = new();

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is LockedPlaceHolderContext model)
            {
                int elementIndex = 1;
                if (model.RequirementsModel.Buildings != null)
                {
                    foreach (var element in model.RequirementsModel.Buildings)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_root);
                        requirementElement.Init(element.Level >= element.RequireLevel, null, $"{element.Name} Level: {element.RequireLevel}");

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

                        requirementElement.Init(element.Count >= element.RequireAmount, null, $"{element.Name} : {DataUtility.ValueToString(element.RequireAmount)}");

                        requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                        rectTransform.SetSiblingIndex(elementIndex);

                        m_requirementViews.Add(requirementElement);

                        elementIndex += 1;
                    }
                }

                if (!model.RequirementsModel.CanUpgrade)
                {
                    m_notMet.SetActive(true);
                    m_unlock.gameObject.SetActive(false);
                }
                else
                {
                    m_notMet.SetActive(false);
                    m_unlock.gameObject.SetActive(true);
                    m_unlock.onClick.AddListener(() =>
                    {
                        Node.TriggerEvent(new UnlockPlaceholderEventArgs(model.Id));
                        Close();
                    });
                }


                base.Show(context, complete);
                resizer.Recalculate();
            }
        }

        public override void Close(Action complete = null)
        {
            foreach (var requirementView in m_requirementViews)
            {
                Destroy(requirementView.gameObject);
            }

            m_requirementViews.Clear();
            m_unlock.onClick.RemoveAllListeners();

            base.Close(complete);
        }
    }
}