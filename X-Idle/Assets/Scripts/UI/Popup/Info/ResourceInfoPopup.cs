using System;
using Data.Model.Popup;
using Data.Utility;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Popup.Info
{
    internal class ResourceInfoPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_resourceName;
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private TMP_Text m_resourceDescription;
        [SerializeField] private TMP_Text m_buildingNameName;
        [SerializeField] private Image m_buildingIcon;
        [SerializeField] private TMP_Text m_buildingDescription;
        [SerializeField] private TMP_Text m_productionLabel;
        [SerializeField] private TMP_Text m_useLabel;
        [SerializeField] private TMP_Text m_capacityLabel;
        [SerializeField] private GameObject m_usageBlock;

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is ResourceInfoContext model)
            {
                m_resourceName.text = model.ResourceName;
                m_resourceIcon.sprite = model.ResourceIcon;
                m_resourceDescription.text = model.ResourceDescription;

                m_buildingNameName.text = model.ProductionBuildingName;
                m_buildingIcon.sprite = model.ProductionBuildingIcon;
                m_buildingDescription.text = $"Primary source of {model.ResourceName.ToLower()}";

                m_productionLabel.text = $"{DataUtility.ValueToString(model.CurrentProduction)}/m";
                if (model.CurrentUse == 0)
                {
                    m_usageBlock.SetActive(false);
                }
                else
                {
                    m_usageBlock.SetActive(true);
                    m_useLabel.text = $"{DataUtility.ValueToString(model.CurrentUse)}/m";
                }

                m_capacityLabel.text = $"{DataUtility.ValueToString(model.CurrentAmount)}/{DataUtility.ValueToString(model.StorageCapacity)}";

                base.Show(context, complete);
            }
        }
    }
}