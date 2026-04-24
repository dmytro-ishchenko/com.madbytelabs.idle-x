using System;

namespace Data.Processor
{
    internal interface IProcessor
    {
        void InitTaskContext(int value);
        void StartProcess();
        void UpdateContext(int value);
        void StopProcess();
        event Action<int> OnProcess;
        bool IsStarted { get; }
    }
}