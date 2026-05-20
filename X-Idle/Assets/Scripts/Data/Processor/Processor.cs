using System;
using System.Threading;
using UnityEngine;

namespace Data.Processor
{
    internal class Processor : IProcessor
    {
        private Awaitable m_process;
        private float m_processValue;
        CancellationTokenSource m_source;

        public event Action OnProcess;
        public bool IsStarted { get; private set; }


        public void StartProcess(float tickTime)
        {
            m_source = new CancellationTokenSource();
            CancellationToken token = m_source.Token;

            IsStarted = true;

            m_process = ProcessData(tickTime, token);
        }


        public void StopProcess()
        {
            if (IsStarted)
            {
                m_source.Cancel();
                m_source.Dispose();
                m_source = null;
            }

            IsStarted = false;
        }

        private async Awaitable ProcessData(float tickTime, CancellationToken token)
        {
            while (true)
            {
                await Awaitable.WaitForSecondsAsync(tickTime, token);
                OnProcess?.Invoke();
            }
        }
    }
}