using UI.Model;

namespace UI.Popup
{
    internal class CreateBuildingPopup : BasePopup
    {
        public override void Show<T>(T context)
        {
            if (context is CreateBuildingContext model)
            {
                base.Show(context);
            }
        }
    }
}