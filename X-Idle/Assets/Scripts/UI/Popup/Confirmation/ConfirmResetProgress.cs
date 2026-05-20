using UI.Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Confirmation
{
    internal class ConfirmResetProgress : BasePopup
    {
        [SerializeField] private Button m_buttonReset;

        public override void Show()
        {
            m_buttonReset.onClick.AddListener(() =>
            {
                Node.TriggerEvent(new ResetProgressEventArgs());
                Close();
            });
            base.Show();
        }

        public override void Close()
        {
            m_buttonReset.onClick.RemoveAllListeners();
            base.Close();
        }
    }
}