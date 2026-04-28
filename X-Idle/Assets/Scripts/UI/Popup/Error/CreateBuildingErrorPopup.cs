using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Data.Model.Popup;
using TMPro;
using UI.Popup.Upgrade;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Error
{
    internal class CreateBuildingErrorPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private Image m_icon;
        [SerializeField] private SerializedDictionary<RequireElementType, RequiredElementView> m_requiredElements;
        [SerializeField] private Transform m_root;

        private readonly List<RequiredElementView> m_requirementViews = new();

        public override void Show<T>(T context)
        {
            if (context is CreateBuildingRequirementsContext model)
            {
                m_name.text = model.Name;
                m_icon.sprite = model.Icon;
                int elementIndex = 1;

                if (model.Buildings != null)
                    foreach (var building in model.Buildings)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Building], m_root);
                        requirementElement.Init(building.Level >= building.RequireLevel, $"{building.Name} Level: {building.RequireLevel}");

                        requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                        rectTransform.SetSiblingIndex(elementIndex);

                        m_requirementViews.Add(requirementElement);

                        elementIndex += 1;
                    }

                if (model.Resources != null)
                    foreach (var resource in model.Resources)
                    {
                        var requirementElement = Instantiate(m_requiredElements[RequireElementType.Resource], m_root);
                        requirementElement.Init(resource.Count >= resource.RequireAmount, $"{resource.Name} : {resource.RequireAmount}");

                        requirementElement.TryGetComponent<RectTransform>(out var rectTransform);
                        rectTransform.SetSiblingIndex(elementIndex);

                        m_requirementViews.Add(requirementElement);

                        elementIndex += 1;
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

            base.Close();
        }
    }
}