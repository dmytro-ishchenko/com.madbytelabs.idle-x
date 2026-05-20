using System;
using UI.Enum;

namespace UI.Event
{
    internal class ShowCurtainsEventArgs
    {
        public ShowCurtainsEventArgs(float time, bool immediately, Action<CurtainsState> onStateChanged)
        {
            Time = time;
            OnStateChanged = onStateChanged;
            Immediately = immediately;
        }

        public float Time { get; }
        public bool Immediately { get; }
        public Action<CurtainsState> OnStateChanged { get; }
    }
}