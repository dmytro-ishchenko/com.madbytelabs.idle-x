using Data.Enum;
using Data.Model;
using Data.Utility;
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
            m_skrapLabel.text = DataUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Scrap));
            m_energyLabel.text = DataUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Energy));
            m_foodLabel.text = DataUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Food));
            m_waterLabel.text = DataUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Water));
            m_partsLabel.text = DataUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Parts));
            m_dataLabel.text = DataUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Data));
        }
    }
}