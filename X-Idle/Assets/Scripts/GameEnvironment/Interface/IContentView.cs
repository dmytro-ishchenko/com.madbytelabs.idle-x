using Common.Pattern.BobbleEvent;
using Data.Model;
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