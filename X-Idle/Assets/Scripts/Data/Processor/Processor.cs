using System;
using UnityEngine;

namespace Data.Processor
{
    internal class Processor : IProcessor
    {
        private Awaitable m_process;
        private float m_processValue;

        public event Action OnProcess;
        public bool IsStarted { get; private set; }


        public void StartProcess(float tickTime)
        {
            IsStarted = true;
            m_process = ProcessData(tickTime);
        }


        public void StopProcess()
        {
            if (IsStarted)
            {
                m_process.Cancel();
            }

            IsStarted = false;
        }

        private async Awaitable ProcessData(float tickTime)
        {
            while (true)
            {
                await Awaitable.WaitForSecondsAsync(tickTime);
                OnProcess?.Invoke();
            }
        }
    }
}