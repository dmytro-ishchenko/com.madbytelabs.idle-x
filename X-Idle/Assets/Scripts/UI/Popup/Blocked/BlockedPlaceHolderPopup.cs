using System;
using Data.Model.Popup;
using TMPro;
using UnityEngine;

namespace UI.Popup.Blocked
{
    internal class BlockedPlaceHolderPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_description;

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is LockedPlaceHolderContext model)
            {
                base.Show(context, complete);
            }
        }

        public override void Close(Action complete = null)
        {
            base.Close(complete);
        }
    }
}