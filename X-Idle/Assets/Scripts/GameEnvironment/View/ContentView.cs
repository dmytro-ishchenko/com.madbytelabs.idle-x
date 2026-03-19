using Common.Events;
using Common.Interface;
using Common.Model;
using Common.Pattern.BobbleEvent;
using UnityEngine;

namespace GameEnvironment.View
{
    internal class ContentView : MonoBehaviour, IContentView
    {
        public GameObject GameObject => gameObject;
        public ContentModel Model { get; private set; }

        public void Init(ContentModel model)
        {
            Model = model;
            Node = new();
        }


        private void OnMouseDown()
        {
            Node.TriggerEvent("SelectContent", new SelectContentEventArgs(Model));
        }

        public Node Node { get; private set; }
    }
}