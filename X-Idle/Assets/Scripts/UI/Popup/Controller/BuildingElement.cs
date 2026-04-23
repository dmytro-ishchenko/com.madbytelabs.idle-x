using System;
using Data.Interface;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Popup.Controller
{
    internal class BuildingElement : MonoBehaviour
    {
        [SerializeField] private Button m_elementButton;
        [SerializeField] private Image m_icon;
        [SerializeField] private Image m_border;
        [SerializeField] private TMP_Text m_name;
        private string m_id;
        public event Action<BuildingElement> OnSelect;
        public string Id => m_id;

        void Awake()
        {
            m_elementButton.onClick.AddListener(() => OnSelect?.Invoke(this));
        }


        public void Init(IBuildingTemplate template)
        {
            m_icon.sprite = template.Icon;
            m_name.text = template.Name;
            m_id = template.Id;
        }

        public void Select(bool select)
        {
            m_border.gameObject.SetActive(select);
        }
    }
}