using System;
using Data.Model.Popup;

namespace UI.Popup.Info
{
    internal class ResourceInfoPopup : BasePopup
    {
        
       
        public override void Show<T>(T context, Action complete = null)
        {
            if (context is  ResourceInfoContext model)
            {
               

                base.Show(context, complete);
              
            }
        }

    }
}