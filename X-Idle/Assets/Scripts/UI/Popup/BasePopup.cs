using Common.Pattern.BobbleEvent;
using UI.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    internal class BasePopup : MonoBehaviour, IPopup, IMonoNode
    {
        [SerializeField] protected ContentSizer resizer;
        [SerializeField] private Button[] m_closeButtons;

        public Node Node { get; } = new();

        protected void Awake()
        {
            foreach (var closeButton in m_closeButtons)
                closeButton.onClick.AddListener(Close);
        }

        public virtual void Show<T>(T context)
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}