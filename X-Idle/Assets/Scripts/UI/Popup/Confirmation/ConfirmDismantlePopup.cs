using System;
using Data.Enum;
using Data.Events;
using TMPro;
using UI.Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Confirmation
{
    internal class ConfirmDismantlePopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private Button m_dismantleButton;

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is ShowDismantleBuildingEventArgs model)
            {
                m_name.text = model.BuildingModel.Template.Name;
                m_icon.sprite = model.BuildingModel.Template.Icon;
                m_level.text = model.BuildingModel.Level.ToString();
                m_dismantleButton.onClick.AddListener(() =>
                {
                    Node.TriggerEvent(new BuildingProcessEventArgs(model.BuildingModel.Id, model.BuildingModel.Template.Id, BuildingActionType.DismantleBuildingRequest));
                    Close();
                });
                base.Show(context, complete);
            }
        }


        public override void Close(Action complete = null)
        {
            ClearPopup();
            base.Close(complete);
        }

        protected override void ClearPopup()
        {
            m_dismantleButton.onClick.RemoveAllListeners();
        }
    }
}