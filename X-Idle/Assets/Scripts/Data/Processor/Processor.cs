using System;
using UnityEngine;

namespace Data.Processor
{
    internal class Processor : IProcessor
    {
        private Awaitable m_process;
        private float m_processValue;

        public event Action<float> OnProcess;
        public bool IsStarted { get; private set; }


        public void InitTaskContext(float value)
        {
            m_processValue = value;
        }

        public void StartProcess()
        {
            IsStarted = true;

            m_process = ProcessData();
        }

        public void UpdateContext(float value)
        {
        }

        public void StopProcess()
        {
            if (IsStarted)
            {
                m_process.Cancel();
            }

            IsStarted = false;
        }

        private async Awaitable ProcessData()
        {
            while (true)
            {
                await Awaitable.WaitForSecondsAsync(1.0f);
                OnProcess?.Invoke(m_processValue);
            }
        }
    }
}