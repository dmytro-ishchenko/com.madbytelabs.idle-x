using System;
using UI.Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Confirmation
{
    internal class ConfirmResetProgress : BasePopup
    {
        [SerializeField] private Button m_buttonReset;

        public override void Show(Action complete = null)
        {
            m_buttonReset.onClick.AddListener(() =>
            {
                Node.TriggerEvent(new ResetProgressEventArgs());
                Close();
            });
            base.Show(complete);
        }

        public override void Close(Action complete = null)
        {
            m_buttonReset.onClick.RemoveAllListeners();
            base.Close(complete);
        }
    }
}