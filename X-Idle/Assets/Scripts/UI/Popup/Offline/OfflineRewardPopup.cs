using System;
using System.Collections.Generic;
using Data.Model.Popup;
using Data.Utility;
using TMPro;
using UI.Config;
using UI.Popup.Info;
using UnityEngine;

namespace UI.Popup.Offline
{
    internal class OfflineRewardPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_timeLabel;
        [SerializeField] private Transform m_root;
        [SerializeField] private InfoElement m_resourceSource;
        private List<InfoElement> m_views = new();

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is OfflineReward model)
            {
                m_timeLabel.text = DataUtility.SecondsToTime(model.Time);

                foreach (var element in model.ResourceRewardModels)
                {
                    var view = Instantiate(m_resourceSource);
                    view.transform.SetParent(m_root);
                    m_views.Add(view);
                    string value = element.Amount > 0 ? $"+{DataUtility.ValueToString(element.Amount)}" : $"-{DataUtility.ValueToString(element.Amount)}";

                    view.Init(element.Icon, element.ResourceName, value,
                        element.Amount > 0 ? UIConfig.Instance.PositiveEffectColor : UIConfig.Instance.NegativeEffectColor);
                    view.gameObject.SetActive(true);
                }

                base.Show(context, complete);
                resizer.Recalculate();
            }
        }

        public override void Close(Action complete = null)
        {
            foreach (var element in m_views)
            {
                Destroy(element.gameObject);
            }

            m_views.Clear();
            base.Close(complete);
        }
    }
}