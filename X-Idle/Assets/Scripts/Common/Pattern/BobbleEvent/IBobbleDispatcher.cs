using System;


namespace Common.Pattern.BobbleEvent
{
    public interface IBobbleDispatcher
    {
        void Subscribe<T>(Action<T> handler) where T : IEventArgs;
        void UnSubscribe<T>(Action<T> handler) where T : IEventArgs;
        void Dispatch<T>(T args) where T : IEventArgs;
    }
}