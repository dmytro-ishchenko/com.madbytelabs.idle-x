using System;
using Data.Model.Popup;
using TMPro;
using UI.Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Select
{
    internal class SelectBuildingPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private Button m_info;
        [SerializeField] private Button m_upgrade;
        [SerializeField] private Button m_demolish;

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is SelectBuildingContext model)
            {
                m_name.text = model.BuildingModel.Template.Name.ToUpper();
                m_icon.sprite = model.BuildingModel.Template.Icon;
                m_level.text = $"Lv.{model.BuildingModel.Level}";

                m_upgrade.onClick.AddListener(() => { Node.TriggerEvent(new ShowUpgradeBuildingEventArgs(model.BuildingModel)); });
                m_info.onClick.AddListener(() => { Node.TriggerEvent(new ShowBuildingsInfoEventArgs(model.BuildingModel)); });
                m_demolish.onClick.AddListener(() => { Node.TriggerEvent(new ShowDismantleBuildingEventArgs(model.BuildingModel)); });

                base.Show(context, complete);
            }
        }

        public override void Close(Action complete = null)
        {
            m_info.onClick.RemoveAllListeners();
            m_upgrade.onClick.RemoveAllListeners();
            m_demolish.onClick.RemoveAllListeners();

            base.Close(complete);
        }
    }
}