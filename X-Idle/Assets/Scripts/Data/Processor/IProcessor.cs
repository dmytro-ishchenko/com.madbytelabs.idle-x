using System;

namespace Data.Processor
{
    internal interface IProcessor
    {
        void InitTaskContext(float value);
        void StartProcess();
        void UpdateContext(float value);
        void StopProcess();
        event Action<float> OnProcess;
        bool IsStarted { get; }
    }
}