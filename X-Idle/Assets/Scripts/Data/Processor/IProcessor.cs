using System;
using Data.Processor.Context;

namespace Data.Processor
{
    internal interface IProcessor
    {
        void StartProcess<T>(T context) where T : IProcessContext;
        void UpdateContext<T>(T context) where T : IProcessContext;
        void StopProcess();

        event Action<int> OnProcess;
    }
}