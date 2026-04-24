using Data.Model;
using UnityEngine;
using TMPro;

namespace UI.Controller
{
    internal class UserInfoViewController : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_skrapLabel;
        [SerializeField] private TMP_Text m_energyLabel;
        [SerializeField] private TMP_Text m_foodLabel;
        [SerializeField] private TMP_Text m_waterLabel;

        public void UpdateUserResources(UserResources userResources)
        {
            m_skrapLabel.text = ConvertValue(userResources.Scrap);
            m_energyLabel.text = ConvertValue(userResources.Energy);
            m_foodLabel.text = ConvertValue(userResources.Food);
            m_waterLabel.text = ConvertValue(userResources.Water);
        }

        string ConvertValue(int value)
        {
            if (value >= 1000000)
            {
                float val = value / 1000000f;
                return val.ToString("0.###");
            }
            else if (value >= 1000)
            {
                float val = value / 1000f;
                return val.ToString("0.###");
            }
            else
            {
                return value.ToString();
            }
        }
    }
}