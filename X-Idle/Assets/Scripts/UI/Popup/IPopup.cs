namespace UI.Popup
{
    internal interface IPopup
    {
        void Show<T>(T context);
        void Close();
    }
}