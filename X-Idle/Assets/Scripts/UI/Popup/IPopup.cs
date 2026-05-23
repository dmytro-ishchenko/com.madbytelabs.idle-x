using System;

namespace UI.Popup
{
    internal interface IPopup
    {
        bool IsActive { get; }
        void Show<T>(T context, Action complete = null);
        void Close(Action complete = null);
        void CloseImmediately();
    }
}