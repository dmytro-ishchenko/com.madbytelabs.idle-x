using Common.Interface;
using UnityEngine;

namespace Data.Interface
{
    public interface IContentTemplate : ITemplate
    {
        
        GameObject View { get; }
        IContentContext GetContentContext<T>() where T : IContentContext;
    }
}