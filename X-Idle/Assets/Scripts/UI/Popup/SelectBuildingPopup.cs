using Data.Model;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Popup
{
    internal class SelectBuildingPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_description;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private Button m_upgrade;

        public override void Show<T>(T context)
        {
            if (context is BuildingModel model)
            {
                m_name.text = model.Template.Name;
                m_level.text = model.Level.ToString();
                m_icon.sprite = model.Template.Icon;
                base.Show(context);
            }
        }
    }
}