using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Data.Processor
{
    internal class Processor : IProcessor
    {
        private Awaitable m_process;
        private int m_processValue;

        public event Action<int> OnProcess;
        public bool IsStarted { get; private set; }


        public void InitTaskContext(int value)
        {
            m_processValue = value;
        }

        public void StartProcess()
        {
            IsStarted = true;

            m_process = ProcessData();
        }

        public void UpdateContext(int value)
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