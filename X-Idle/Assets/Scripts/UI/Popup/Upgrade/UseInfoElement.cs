using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Upgrade
{

    public class UseInfoElement:MonoBehaviour
    {
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private TMP_Text m_current;
        [SerializeField] private TMP_Text m_next;

        public void Init(Sprite icon, string name, string current, string next)
        {
            if (icon != null)
                m_icon.sprite = icon;
            else
            {
                m_icon.gameObject.SetActive(false);
            }

            m_name.text =name;
            m_current.text = current;
            m_next.text = next;
        }
    }
}