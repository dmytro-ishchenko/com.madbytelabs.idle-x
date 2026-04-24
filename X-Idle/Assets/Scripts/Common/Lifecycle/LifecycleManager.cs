using System.Collections.Generic;
using Commom.Pattern;
using Common.Lifecycle;

namespace Commom.Lifecycle
{
    public class LifecycleManager : MonoSingleton<LifecycleManager>
    {
        private List<ILifecycleDelegate> m_subscribers;

        protected override void Awake()
        {
            base.Awake();
            m_subscribers = new List<ILifecycleDelegate>();
        }

        protected override void OnApplicationQuit()
        {
            InvokeApplicationQuit();
            base.OnApplicationQuit();
        }

        private void OnApplicationPause(bool pauseStatus) => InvokeApplicationPause(pauseStatus);

        private void OnApplicationFocus(bool hasFocus) => InvokeApplicationFocus(hasFocus);

        private void InvokeApplicationQuit()
        {
            foreach (var subscriber in m_subscribers)
                subscriber.OnApplicationQuit();
        }

        private void InvokeApplicationPause(bool pauseStatus)
        {
            foreach (var subscriber in m_subscribers)
                subscriber.OnApplicationPause(pauseStatus);
        }

        private void InvokeApplicationFocus(bool hasFocus)
        {
            foreach (var subscriber in m_subscribers)
                subscriber.OnApplicationFocus(hasFocus);
        }

        public void AddDelegate(ILifecycleDelegate del)
        {
            m_subscribers.Add(del);
        }

        public void RemoveDelegate(ILifecycleDelegate del)
        {
            for (var i = m_subscribers.Count - 1; i >= 0; i--)
            {
                if (m_subscribers[i] != del)
                    continue;

                m_subscribers.RemoveAt(i);
                break;
            }
        }
    }
}