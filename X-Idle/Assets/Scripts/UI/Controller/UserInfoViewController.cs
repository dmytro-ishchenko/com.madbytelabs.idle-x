using Common.Pattern.BobbleEvent;
using Data.Enum;
using Data.Model;
using Data.Utility;
using UnityEngine;
using TMPro;
using UI.Event;
using UnityEngine.UI;

namespace UI.Controller
{
    internal class UserInfoViewController : MonoNode
    {
        [SerializeField] private TMP_Text m_skrapLabel;
        [SerializeField] private TMP_Text m_energyLabel;
        [SerializeField] private TMP_Text m_foodLabel;
        [SerializeField] private TMP_Text m_waterLabel;
        [SerializeField] private TMP_Text m_dataLabel;
        [SerializeField] private TMP_Text m_partsLabel;

        [SerializeField] private Button m_skrapButton;
        [SerializeField] private Button m_energyButton;
        [SerializeField] private Button m_foodButton;
        [SerializeField] private Button m_waterButton;
        [SerializeField] private Button m_dataButton;
        [SerializeField] private Button m_partsButton;

        void Awake()
        {
            m_skrapButton.onClick.AddListener(() => { Node.TriggerEvent(new OpenResourcesInfoEventArgs(GameResourceType.Scrap)); });
            m_energyButton.onClick.AddListener(() => { Node.TriggerEvent(new OpenResourcesInfoEventArgs(GameResourceType.Energy)); });
            m_foodButton.onClick.AddListener(() => { Node.TriggerEvent(new OpenResourcesInfoEventArgs(GameResourceType.Food)); });
            m_waterButton.onClick.AddListener(() => { Node.TriggerEvent(new OpenResourcesInfoEventArgs(GameResourceType.Water)); });
            m_dataButton.onClick.AddListener(() => { Node.TriggerEvent(new OpenResourcesInfoEventArgs(GameResourceType.Data)); });
            m_partsButton.onClick.AddListener(() => { Node.TriggerEvent(new OpenResourcesInfoEventArgs(GameResourceType.Parts)); });
        }

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