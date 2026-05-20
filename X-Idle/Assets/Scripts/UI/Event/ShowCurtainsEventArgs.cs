using System;

namespace UI.Event
{
    public class ShowCurtainsEventArgs
    {
        public ShowCurtainsEventArgs(float time, Action complete)
        {
            Time = time;
            Complete = complete;
        }
        public float Time { get; }
        public Action Complete { get; }
    }
}