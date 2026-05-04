using Data.Enum;
using Data.Model.Popup;
using TMPro;
using UI.Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Select
{
    internal class SelectBuildingPopup : BasePopup
    {
        [SerializeField] private TMP_Text m_name;
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private TMP_Text m_productionLabel;
        [SerializeField] private TMP_Text m_output;
        [SerializeField] private Button m_info;
        [SerializeField] private Button m_upgrade;
        [SerializeField] private Button m_demolish;

        public override void Show<T>(T context)
        {
            if (context is SelectBuildingContext model)
            {
                m_name.text = model.BuildingModel.Template.Name;
                m_icon.sprite = model.BuildingModel.Template.Icon;
                m_level.text = model.BuildingModel.Level.ToString();

                switch (model.BuildingModel.Template.BuildingContext.BuildingType)
                {
                    case BuildingType.MainBuilding:
                        m_productionLabel.gameObject.SetActive(false);
                        m_output.gameObject.SetActive(false);
                        break;
                    case BuildingType.Warehouse:
                        m_productionLabel.text = "Bonus:";
                        m_output.text = "Storage";
                        m_productionLabel.gameObject.SetActive(true);
                        m_output.gameObject.SetActive(true);
                        break;
                    default:
                        m_productionLabel.text = "Output:";
                        m_output.text = model.BuildingModel.Template.BuildingContext.BuildingProduction.ResourcesTemplate.Name;
                        m_productionLabel.gameObject.SetActive(true);
                        m_output.gameObject.SetActive(true);
                        break;
                }


                m_upgrade.onClick.AddListener(() => { Node.TriggerEvent(new ShowUpgradeBuildingEventArgs(model.BuildingModel)); });
                m_info.onClick.AddListener(() => { Node.TriggerEvent(new ShowBuildingsInfoEventArgs(model.BuildingModel)); });
                m_demolish.onClick.AddListener(() => { Node.TriggerEvent(new ShowDemolishBuildingEventArgs(model.BuildingModel)); });

                base.Show(context);
            }
        }

        public override void Close()
        {
            m_info.onClick.RemoveAllListeners();
            m_upgrade.onClick.RemoveAllListeners();
            m_demolish.onClick.RemoveAllListeners();

            base.Close();
        }
    }
}