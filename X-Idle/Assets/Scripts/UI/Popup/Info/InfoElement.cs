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

        public void Init(Sprite icon, string text, string value)
        {
            if (icon != null)
                m_icon.sprite = icon;
            else
            {
                m_icon.gameObject.SetActive(false);
            }

            m_name.text = text;
            m_value.text = value;
        }

        public void InitValue(string value)
        {
            m_value.text = value;
        }
    }
}