using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    internal class BasePopup : MonoBehaviour, IPopup, IMonoNode
    {
        [SerializeField] private Button[] m_closeButtons;


        [SerializeField] List<ContentSizeFitter> m_contentSizeFitters = new();
        public Node Node { get; } = new();

        protected void Awake()
        {
            foreach (var closeButton in m_closeButtons)
                closeButton.onClick.AddListener(Close);
        }

        public virtual void Show<T>(T context)
        {
           // InitContentSize(false);
            InitVerticalLayoutGroup(false);
            gameObject.SetActive(true);
           // InitContentSize(true);
            InitVerticalLayoutGroup(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        protected void InitContentSize(bool enable)
        {
            if (m_contentSizeFitters.Count > 0)
                foreach (var fitter in m_contentSizeFitters)
                {
                    fitter.enabled = enable;
                }
        }

        protected void InitVerticalLayoutGroup(bool enable)
        {
            if (m_contentSizeFitters.Count > 0)
                foreach (var fitter in m_contentSizeFitters)
                {
                    if (fitter.TryGetComponent(out VerticalLayoutGroup group))
                        group.enabled = enable;
                }
        }
    }
}