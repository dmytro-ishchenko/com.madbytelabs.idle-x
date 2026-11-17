using System;
using UnityEngine;
using TMPro;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

namespace UI.Popup.Upgrade
{
    [Serializable]
    internal class RequiredElementView : MonoBehaviour
    {
        [SerializeField] private Color m_successColor;
        [SerializeField] private Color m_failureColor;
        [SerializeField] private GameObject m_root;
        [SerializeField] private Image m_iconMet;
        [SerializeField] private Image m_iconNotMet;
        [SerializeField] private TMP_Text m_lable;

        public void Disable()
        {
            m_root.SetActive(false);
        }

        public void Init(bool buildingFollow,  string message)
        {
            if (buildingFollow)
            {
                m_lable.color = m_successColor;
                m_iconMet.gameObject.SetActive(true);
                m_iconNotMet.gameObject.SetActive(false);
            }
            else
            {
                m_lable.color = m_failureColor;
                m_iconMet.gameObject.SetActive(false);
                m_iconNotMet.gameObject.SetActive(true);
            }

            m_lable.text = message;
            m_root.SetActive(true);
        }
    }
}