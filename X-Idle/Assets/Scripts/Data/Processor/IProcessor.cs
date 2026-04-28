using System;

namespace Data.Processor
{
    internal interface IProcessor
    {
        void StartProcess();
        void StopProcess();
        event Action OnProcess;
        bool IsStarted { get; }
    }
}