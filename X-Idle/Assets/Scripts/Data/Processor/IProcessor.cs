using System;

namespace Data.Processor
{
    internal interface IProcessor
    {
        void StartProcess(float tickTime);
        void StopProcess();
        event Action OnProcess;
        bool IsStarted { get; }
    }
}