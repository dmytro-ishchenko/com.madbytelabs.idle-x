using Data.Model.Popup;
using TMPro;
using UnityEngine;

namespace UI.Popup.Blocked
{
    internal class BlockedPlaceHolderPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_description;

        public override void Show<T>(T context)
        {
            if (context is LockedPlaceHolderContext model)
            {
                base.Show(context);
            }
        }

        public override void Close()
        {
            base.Close();
        }
    }
}