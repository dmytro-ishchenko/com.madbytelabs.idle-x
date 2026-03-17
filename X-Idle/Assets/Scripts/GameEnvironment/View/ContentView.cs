using Common.Interface;
using Common.Model;
using UnityEngine;

namespace GameEnvironment.View
{
    internal class ContentView : MonoBehaviour, IContentView
    {
        public GameObject GameObject => gameObject;
        public void Init(ContentModel model)
        {
            
        }
    }
}