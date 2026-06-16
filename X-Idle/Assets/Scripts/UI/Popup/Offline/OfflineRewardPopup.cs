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
        [SerializeField] private BlockModel m_resourcesBlock;
        [SerializeField] private BlockModel m_storageBlock;

        private List<InfoElement> m_views = new();

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is OfflineReward model)
            {
                m_timeLabel.text = DataUtility.SecondsToTime(model.Time);

                if (model.ResourceRewardModels.Count == 0)
                {
                    m_resourcesBlock.Panel.SetActive(false);
                }
                else
                {
                    m_resourcesBlock.Panel.SetActive(true);
                    foreach (var element in model.ResourceRewardModels)
                    {
                        var view = Instantiate(m_resourcesBlock.ResourceSource);
                        view.transform.SetParent(m_resourcesBlock.Root.transform);
                        m_views.Add(view);
                        string value = element.Amount > 0 ? $"+{DataUtility.ValueToString(element.Amount)}" : $"-{DataUtility.ValueToString(element.Amount)}";

                        view.Init(element.Icon, element.ResourceName, value,
                            element.Amount > 0 ? UIConfig.Instance.PositiveEffectColor : UIConfig.Instance.NegativeEffectColor);
                        view.gameObject.SetActive(true);
                    }
                }

                if (model.ResourcesReachLimit.Count == 0)
                {
                    m_storageBlock.Panel.SetActive(false);
                }
                else
                {
                    m_storageBlock.Panel.SetActive(true);
                    foreach (var element in model.ResourcesReachLimit)
                    {
                        var view = Instantiate(m_storageBlock.ResourceSource);
                        view.transform.SetParent(m_storageBlock.Root.transform);
                        m_views.Add(view);
                        view.Init(element.Icon, $"Storage for {element.ResourceName} reach limit", string.Empty, UIConfig.Instance.NegativeEffectColor);
                        view.gameObject.SetActive(true);
                    }
                }

                base.Show(context, complete);
                resizer.Recalculate();
            }
        }

        public override void Close(Action complete = null)
        {
            ClearPopup();
            base.Close(complete);
        }

        protected override void ClearPopup()
        {
            foreach (var element in m_views)
            {
                Destroy(element.gameObject);
            }

            m_views.Clear();
        }


        [Serializable]
        class BlockModel
        {
            public Transform Root;
            public InfoElement ResourceSource;
            public GameObject Panel;
        }
    }
}