using System;
using System.Collections;
using DG.Tweening;
using UI.Enum;
using UI.Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Curtains
{
    internal class CurtainsPopup : BasePopup
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private float m_openCloseTime;
        [SerializeField] private RectTransform m_topPart;
        [SerializeField] private RectTransform m_bottomPart;


        public override void Show<T>(T context, Action complete = null)
        {
            if (context is ShowCurtainsEventArgs args)
            {
                m_slider.value = 0;
                ShowCurtains(args, complete);
            }
        }

        void ShowCurtains(ShowCurtainsEventArgs args, Action complete)
        {
            var topTargetPosition = new Vector2(m_topPart.anchoredPosition.x, 0);
            var bottomTargetPosition = new Vector2(m_bottomPart.anchoredPosition.x, 0);

            if (args.Immediately)
            {
                m_topPart.anchoredPosition = topTargetPosition;
                m_bottomPart.anchoredPosition = bottomTargetPosition;
                EnablePopup(true);
                StartCoroutine(ShowSlider(args, complete));
            }
            else
            {
                m_topPart.anchoredPosition = new Vector2(m_topPart.anchoredPosition.x, (Screen.height / 2f)*1.2f);
                m_bottomPart.anchoredPosition = new Vector2(m_bottomPart.anchoredPosition.x, (-Screen.height / 2f)*1.2f);
                EnablePopup(true);

                m_topPart.DOAnchorPos(topTargetPosition, m_openCloseTime);
                m_bottomPart.DOAnchorPos(bottomTargetPosition, m_openCloseTime).OnComplete(() => { StartCoroutine(ShowSlider(args, complete)); });
            }
        }

        IEnumerator ShowSlider(ShowCurtainsEventArgs args, Action complete)
        {
            float cashedTime = args.Time;
            while (cashedTime > 0)
            {
                cashedTime -= Time.deltaTime;
                yield return null;
                m_slider.value = (args.Time - cashedTime) / args.Time;
            }

            CloseCurtains(args, complete);
        }

        void CloseCurtains(ShowCurtainsEventArgs args, Action complete)
        {
            var topTargetPosition = new Vector2(m_topPart.anchoredPosition.x, (Screen.height / 2f)*1.2f);
            var bottomTargetPosition = new Vector2(m_bottomPart.anchoredPosition.x, (-Screen.height / 2f)*1.2f);

            m_topPart.DOAnchorPos(topTargetPosition, m_openCloseTime);
            m_bottomPart.DOAnchorPos(bottomTargetPosition, m_openCloseTime).OnComplete(() =>
            {
                EnablePopup(false);
                args.OnStateChanged?.Invoke(CurtainsState.Close);
                complete?.Invoke();
            });
        }
    }
}