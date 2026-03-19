using Common.Model;
using Common.Pattern.BobbleEvent;
using UnityEngine;

namespace UI.Controller
{
    internal class GameUIController : MonoBehaviour, IGameUIController
    {
        void Awake()
        {
            Node = new();
        }

        public void ShowSelectContentPopup(ContentModel contentModel)
        {
        }

        public Node Node { get; private set; }
    }
}