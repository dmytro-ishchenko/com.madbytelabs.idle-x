using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Pattern.BobbleEvent
{
    public class BobbleDispatcher : IBobbleDispatcher
    {
        private Dictionary<Type, List<Delegate>> m_eventHandlers = new();

        public void Subscribe<T>(Action<T> handler) where T : IEventArgs
        {
            if (!m_eventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                m_eventHandlers[typeof(T)] = new List<Delegate> { handler };
            }
            else
            {
                handlers.Add(handler);
            }
        }

        public void UnSubscribe<T>(Action<T> handler) where T : IEventArgs
        {
            if (m_eventHandlers.TryGetValue(typeof(T), out List<Delegate> existingHandlers))
            {
                existingHandlers.Remove(handler);
                if (existingHandlers.Count == 0)
                {
                    m_eventHandlers.Remove(typeof(T));
                }
            }
        }

        public void Dispatch<T>(T payload) where T : IEventArgs
        {
            if (m_eventHandlers.TryGetValue(typeof(T), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((Action<T>)handler)?.Invoke(payload);
                }
            }
        }
    }
}