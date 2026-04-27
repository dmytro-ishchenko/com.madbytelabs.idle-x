using Data.Enum;
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
        [SerializeField] private TMP_Text m_partsLabel;
        [SerializeField] private TMP_Text m_dataLabel;

        public void UpdateUserResources(UserResources userResources)
        {
            m_skrapLabel.text = ConvertValue(userResources.GetGameResourceValue(GameResourceType.Scrap));
            m_energyLabel.text = ConvertValue(userResources.GetGameResourceValue(GameResourceType.Energy));
            m_foodLabel.text = ConvertValue(userResources.GetGameResourceValue(GameResourceType.Food));
            m_waterLabel.text = ConvertValue(userResources.GetGameResourceValue(GameResourceType.Water));
            m_partsLabel.text = ConvertValue(userResources.GetGameResourceValue(GameResourceType.Parts));
            m_dataLabel.text = ConvertValue(userResources.GetGameResourceValue(GameResourceType.Data));
        }

        string ConvertValue(float value)
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