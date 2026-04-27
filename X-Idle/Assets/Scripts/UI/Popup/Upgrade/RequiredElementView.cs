using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Popup.Upgrade
{
    [Serializable]
    internal class RequiredElementView : MonoBehaviour
    {
        [SerializeField] private Color m_successColor;
        [SerializeField] private Color m_failureColor;
        [SerializeField] private GameObject m_root;
        [SerializeField] private TMP_Text m_lable;
        [SerializeField] private Toggle m_enough;

        public void Disable()
        {
            m_root.SetActive(false);
        }

        public void Init(bool buildingFollow, string message)
        {
            if (buildingFollow)
            {
                m_lable.color = m_successColor;
                m_enough.isOn = true;
            }
            else
            {
                m_lable.color = m_failureColor;
                m_enough.isOn = false;
            }

            m_lable.text = message;
            m_root.SetActive(true);
        }
    }
}