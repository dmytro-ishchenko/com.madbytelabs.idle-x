using System;
using Common.Pattern.BobbleEvent;
using UI.Controller;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.Popup
{
    internal class BasePopup : MonoBehaviour, IPopup, IMonoNode
    {
        [SerializeField] private OpenContext m_openContext;
        [SerializeField] protected ContentSizer resizer;
        [SerializeField] private Button[] m_closeButtons;
        private CanvasGroup m_canvasGroup;
        private Vector2 m_targetPosition = Vector2.zero;
        public Node Node { get; } = new();

        protected void Awake()
        {
            m_canvasGroup = gameObject.GetComponent<CanvasGroup>();
            foreach (var closeButton in m_closeButtons)
                closeButton.onClick.AddListener(() => { Close(); });
        }

        public virtual void Show(Action complete = null)
        {
            if (m_targetPosition == Vector2.zero)
                m_targetPosition = m_openContext.RectTransform.anchoredPosition;
            ShowPopup(complete);
        }

        public bool IsActive => m_canvasGroup.interactable;

        public virtual void Show<T>(T context, Action complete = null)
        {
            if (m_targetPosition == Vector2.zero)
                m_targetPosition = m_openContext.RectTransform.anchoredPosition;
            ShowPopup(complete);
        }

        public virtual void Close(Action complete = null)
        {
            ClosePopup(complete);
        }

        public void CloseImmediately()
        {
            m_openContext.RectTransform.anchoredPosition = m_targetPosition;
            EnablePopup(false);
        }

        void ShowPopup(Action complete)
        {
            if (m_openContext.OpenDirection != Direction.None)
            {
                switch (m_openContext.OpenDirection)
                {
                    case Direction.Down:
                        m_openContext.RectTransform.anchoredPosition = new Vector2(m_targetPosition.x, m_targetPosition.y + Screen.height);
                        break;
                    case Direction.Up:
                        m_openContext.RectTransform.anchoredPosition = new Vector2(m_targetPosition.x, m_targetPosition.y - Screen.height);
                        break;
                    case Direction.Left:
                        m_openContext.RectTransform.anchoredPosition = new Vector2(m_targetPosition.x + Screen.width, m_targetPosition.y);
                        break;
                    case Direction.Right:
                        m_openContext.RectTransform.anchoredPosition = new Vector2(m_targetPosition.x - Screen.width, m_targetPosition.y);
                        break;
                }


                EnablePopup(true);
                m_openContext.RectTransform.DOAnchorPos(m_targetPosition, m_openContext.OpenTime).OnComplete(() => { complete?.Invoke(); });
            }
            else
            {
                EnablePopup(true);
                complete?.Invoke();
            }
        }

        void ClosePopup(Action complete)
        {
            if (m_openContext.OpenDirection != Direction.None)
            {
                Vector2 closePosition = Vector2.zero;

                switch (m_openContext.OpenDirection)
                {
                    case Direction.Down:
                        closePosition = new Vector2(m_openContext.RectTransform.anchoredPosition.x, m_openContext.RectTransform.anchoredPosition.y + Screen.height);
                        break;
                    case Direction.Up:
                        closePosition = new Vector2(m_openContext.RectTransform.anchoredPosition.x, m_openContext.RectTransform.anchoredPosition.y - Screen.height);
                        break;
                    case Direction.Left:
                        closePosition = new Vector2(m_openContext.RectTransform.anchoredPosition.x + Screen.width, m_openContext.RectTransform.anchoredPosition.y);
                        break;
                    case Direction.Right:
                        closePosition = new Vector2(m_openContext.RectTransform.anchoredPosition.x - Screen.width, m_openContext.RectTransform.anchoredPosition.y);
                        break;
                }

                m_openContext.RectTransform.DOAnchorPos(closePosition, m_openContext.OpenTime).OnComplete(() =>
                {
                    complete?.Invoke();
                    EnablePopup(false);
                    m_openContext.RectTransform.anchoredPosition = m_targetPosition;
                });
            }
            else
            {
                EnablePopup(false);
                complete?.Invoke();
            }
        }

        void EnablePopup(bool enable)
        {
            m_canvasGroup.alpha = enable ? 1 : 0;
            m_canvasGroup.blocksRaycasts = enable;
            m_canvasGroup.interactable = enable;
        }

        [Serializable]
        class OpenContext
        {
            public RectTransform RectTransform;
            public float OpenTime = 0.3f;
            public Direction OpenDirection;
            public bool UseZoom;
        }

        enum Direction
        {
            None,
            Left,
            Right,
            Up,
            Down,
        }
    }
}