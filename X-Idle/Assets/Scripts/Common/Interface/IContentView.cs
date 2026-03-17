using Common.Model;
using UnityEngine;

namespace Common.Interface
{
    public interface IContentView
    {
        GameObject GameObject { get; }
        void Init(ContentModel model);
    }
}