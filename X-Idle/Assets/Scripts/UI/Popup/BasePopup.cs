using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    internal class BasePopup : MonoBehaviour, IPopup
    {
        [SerializeField] private Button[] m_closeButtons;

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