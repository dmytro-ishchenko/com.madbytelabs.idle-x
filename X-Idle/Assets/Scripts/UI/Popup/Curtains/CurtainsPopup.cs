using System;
using System.Collections;
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
        private CanvasGroup m_canvasGroup;

        public override void Show<T>(T context, Action complete = null)
        {
            if (m_canvasGroup == null)
                m_canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (context is ShowCurtainsEventArgs args)
            {
                m_canvasGroup.alpha = 0;
                gameObject.SetActive(true);
                m_slider.value = 0;

                base.Show(context, complete);

                StartCoroutine(WaitAndClose(args));
            }
        }

        IEnumerator WaitAndClose(ShowCurtainsEventArgs args)
        {
            float cashedTime = args.Time;
            if (args.Immediately)
            {
                m_canvasGroup.alpha = 1;
            }
            else
            {
                while (m_canvasGroup.alpha < 1)
                {
                    m_canvasGroup.alpha += Time.deltaTime / m_openCloseTime;
                    yield return null;
                }
            }

            while (cashedTime > 0)
            {
                cashedTime -= Time.deltaTime;
                yield return null;
                m_slider.value = (args.Time - cashedTime) / args.Time;
            }

            while (m_canvasGroup.alpha > 0)
            {
                m_canvasGroup.alpha -= Time.deltaTime / m_openCloseTime;
                yield return null;
            }

            Close();
            args.OnStateChanged?.Invoke(CurtainsState.Close);
        }
    }
}