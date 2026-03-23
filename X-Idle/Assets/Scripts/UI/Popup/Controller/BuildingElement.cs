using Data.Interface;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Popup.Controller
{
    internal class BuildingElement : MonoBehaviour
    {
        [SerializeField] Image m_icon;
        [SerializeField] TMP_Text m_name;

        public void Init(IBuildingTemplate template)
        {
            m_icon.sprite = template.Icon;
            m_name.text = template.Name;
        }
    }
}