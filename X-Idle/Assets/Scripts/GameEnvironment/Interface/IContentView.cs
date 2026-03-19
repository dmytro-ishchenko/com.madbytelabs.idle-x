using Common.Model;
using Common.Pattern.BobbleEvent;
using UnityEngine;

namespace GameEnvironment.Interface
{
    public interface IContentView : IMonoNode
    {
        GameObject GameObject { get; }

        ContentModel Model { get; }
        void Init(ContentModel model);
    }
}