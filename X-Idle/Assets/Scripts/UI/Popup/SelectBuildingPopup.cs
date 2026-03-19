using Data.Model;

namespace UI.Popup
{
    internal class SelectBuildingPopup : BasePopup
    {
        public override void Show<T>(T context)
        {
            if (context is BuildingModel model)
            {
                base.Show(context);
            }
        }
    }
}