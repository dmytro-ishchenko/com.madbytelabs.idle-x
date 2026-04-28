using Data.Enum;
using Data.Model;
using UnityEngine;
using TMPro;
using UI.Utility;

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
            m_skrapLabel.text = UiUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Scrap));
            m_energyLabel.text = UiUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Energy));
            m_foodLabel.text = UiUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Food));
            m_waterLabel.text = UiUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Water));
            m_partsLabel.text = UiUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Parts));
            m_dataLabel.text = UiUtility.ValueToString(userResources.GetGameResourceValue(GameResourceType.Data));
        }
    }
}