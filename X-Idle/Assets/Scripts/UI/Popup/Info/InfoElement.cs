using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Popup.Info
{
    internal class InfoElement : MonoBehaviour
    {
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_value;

        public void Init(Sprite icon, string text, string value, Color color)
        {
            if (icon != null)
                m_icon.sprite = icon;
            else
            {
                m_icon.gameObject.SetActive(false);
            }

            m_name.text = text;
            m_value.text = value;
            m_value.color = color;
        }

        public void InitValue(string value, Color color)
        {
            m_value.text = value;
            m_value.color = color;
        }
    }
}