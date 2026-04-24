using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Processor.Context;

namespace Data.Processor
{
    internal abstract class Processor : IProcessor
    {
        private Task m_processTask;
        private int m_processValue;
        private CancellationTokenSource m_cancellationTokenSource;
        public event Action<int> OnProcess;

        public abstract void StartProcess<T>(T context) where T : IProcessContext;
        public abstract void UpdateContext<T>(T context) where T : IProcessContext;

        public void StopProcess()
        {
            if (m_processTask != null)
            {
                m_cancellationTokenSource?.Cancel();
                m_cancellationTokenSource?.Dispose();
                m_processTask = null;
            }
        }

        protected void InitTaskContext(int value)
        {
            m_processValue = value;
        }

        protected void StartProcess()
        {
            m_processTask = Task.Run(ProcessData);
        }

        private async Task ProcessData()
        {
            m_cancellationTokenSource = new CancellationTokenSource();
            while (true)
            {
                await Task.Delay(1000, m_cancellationTokenSource.Token);
                OnProcess?.Invoke(m_processValue);
            }
        }
    }
}