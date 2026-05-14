using Common.Pattern.BobbleEvent;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    internal class BasePopup : MonoBehaviour, IPopup, IMonoNode
    {
        [SerializeField] protected RectTransform[] m_updateList;
        [SerializeField] protected bool m_resizeToContent = true;
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

        protected void UpdateContentHolder(RectTransform holder)
        {
            float height = 0;
            foreach (RectTransform rectTransform in holder)
            {
                height += rectTransform.rect.height;
            }

            holder.sizeDelta = new Vector2(holder.sizeDelta.x, height);
        }
    }
}