using System.Collections.Generic;
using Data.Model.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Info
{
    internal class BuildingInfoPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private InfoElement m_infoElement;
        [SerializeField] private Transform m_effectRoot;
        [SerializeField] private GameObject m_storageBlock;
        [SerializeField] private Transform m_storageRoot;
        [SerializeField] private GameObject m_conditionBlock;
        [SerializeField] private InfoElement m_efficiencyElement;
        [SerializeField] private Button[] m_buttons;
        private readonly List<InfoElement> m_elements = new();

        public override void Show<T>(T context)
        {
            if (context is SelectBuildingInfo model)
            {
                TryGetComponent(out CanvasGroup group);
                group.interactable = false;
                group.blocksRaycasts = false;
                group.alpha = 0;

                m_name.text = model.Name.ToUpper();
                m_level.text = $"Lv.{model.Level}";
                m_description.text = model.Description;
                m_icon.sprite = model.Icon;

                InfoElement view = null;

                foreach (var effect in model.Effects)
                {
                    view = Instantiate(m_infoElement, m_effectRoot);
                    view.Init(effect.Icon, $"{effect.Text} :", $"{effect.Value}");
                    m_elements.Add(view);
                    view.gameObject.SetActive(true);
                }

                if (model.Storage != null)
                {
                    m_storageBlock.gameObject.SetActive(true);

                    view = Instantiate(m_infoElement, m_storageRoot);
                    view.Init(model.Storage.Icon, $"{model.Storage.Text} :", $"{model.Storage.Value}");
                    m_elements.Add(view);
                    view.gameObject.SetActive(true);
                }
                else
                {
                    m_storageBlock.gameObject.SetActive(false);
                }

                if (model.Condition != null)
                {
                    m_conditionBlock.gameObject.SetActive(true);
                    m_efficiencyElement.InitValue($"{model.Condition.Value}");
                    m_efficiencyElement.gameObject.SetActive(true);
                }
                else
                {
                    m_conditionBlock.gameObject.SetActive(false);
                }

                group.interactable = true;
                group.blocksRaycasts = true;
                group.alpha = 1;

                base.Show(context);
            }
        }

        public override void Close()
        {
            TryGetComponent(out CanvasGroup group);
            group.interactable = false;
            group.blocksRaycasts = false;
            group.alpha = 0;

            foreach (var element in m_elements)
            {
                Destroy(element.gameObject);
            }

            m_elements.Clear();

            base.Close();
        }
    }
}