using UnityEngine;

namespace Common.Pattern.BobbleEvent
{
    public class MonoNode : MonoBehaviour, IMonoNode
    {
        public Node Node { get; } = new();
    }
}