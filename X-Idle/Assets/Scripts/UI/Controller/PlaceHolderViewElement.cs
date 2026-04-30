using Common.Pattern.BobbleEvent;
using Data.Enum;
using Data.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controller
{
    internal class PlaceHolderViewElement : MonoNode
    {
        [SerializeField] private Button m_button;

        public void Init(string id, PlaceHolderStatus status)
        {
            m_button.onClick.AddListener(() => { Node.TriggerEvent(new SelectPlaceHolderEventArgs(id, status)); });
        }
    }
}