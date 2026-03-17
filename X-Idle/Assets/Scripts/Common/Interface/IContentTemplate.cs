using UnityEngine;

namespace Common.Interface
{
    public interface IContentTemplate : ITemplate
    {
        
        GameObject View { get; }
        IContentContext GetContentContext<T>() where T : IContentContext;
    }
}