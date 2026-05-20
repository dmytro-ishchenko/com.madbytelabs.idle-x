using System.Collections;
using UI.Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Curtains
{
    internal class CurtainsPopup : BasePopup
    {
        [SerializeField] private Slider m_slider;

        public override void Show<T>(T context)
        {
            if (context is ShowCurtainsEventArgs args)
            {
                gameObject.SetActive(true);
                m_slider.value = 0;

                base.Show(context);

                StartCoroutine(WaitAndClose(args));
            }
        }

        IEnumerator WaitAndClose(ShowCurtainsEventArgs args)
        {
            float cashedTime = args.Time;

            while (cashedTime > 0)
            {
                cashedTime -= Time.deltaTime;
                yield return null;
                m_slider.value = (args.Time - cashedTime) / args.Time;
            }

            Close();
            args.Complete?.Invoke();
        }
    }
}