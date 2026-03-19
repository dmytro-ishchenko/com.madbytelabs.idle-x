using System;
using Common.Events;

namespace GameEnvironment
{
    public interface IEnvironment
    {
        event Action<SelectContentEventArgs> OnSelectContent;
    }
}