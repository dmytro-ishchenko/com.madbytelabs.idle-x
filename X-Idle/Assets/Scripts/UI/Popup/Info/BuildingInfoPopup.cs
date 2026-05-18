using System.Collections.Generic;
using Data.Model.Popup;
using TMPro;
using UI.Config;
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
            m_name.text = model.Name.ToUpper();
                m_level.text = $"Lv.{model.Level}";
                m_description.text = model.Description;
                m_icon.sprite = model.Icon;

                InfoElement view = null;

                foreach (var effect in model.Effects)
                {
                    view = Instantiate(m_infoElement, m_effectRoot);
                    view.Init(effect.Icon, $"{effect.Text}", $"{effect.Value}", effect.IsPositive ? UIConfig.Instance.PositiveEffectColor : UIConfig.Instance.NegativeEffectColor);
                    m_elements.Add(view);
                    view.gameObject.SetActive(true);
                }

                if (model.Storage != null)
                {
                    m_storageBlock.gameObject.SetActive(true);

                    view = Instantiate(m_infoElement, m_storageRoot);
                    view.Init(model.Storage.Icon, $"{model.Storage.Text}", $"{model.Storage.Value}", UIConfig.Instance.PositiveBonusColor);
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

                    Color color = UIConfig.Instance.Condition100;

                    if (model.Condition.Value == 0)
                        color = UIConfig.Instance.Condition0;
                    else if (model.Condition.Value < 30)
                        color = UIConfig.Instance.Condition30;
                    else if (model.Condition.Value < 60)
                        color = UIConfig.Instance.Condition60;
                    else if (model.Condition.Value < 90)
                        color = UIConfig.Instance.Condition90;

                    m_efficiencyElement.InitValue($"{model.Condition.Value}%", color);
                    m_efficiencyElement.gameObject.SetActive(true);
                }
                else
                {
                    m_conditionBlock.gameObject.SetActive(false);
                }

                base.Show(context);
                resizer.Recalculate();
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