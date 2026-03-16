using Data.ContentLibrary.Templates.Context;
using UnityEngine;

namespace Data.ContentLibrary.Templates
{
    public interface IContentTemplate : ITemplate
    {
        
        GameObject View { get; }
        T GetContentContext<T>() where T : ContentContext;
    }
}