using System;
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
        [SerializeField] private TMP_Text m_level;
        [SerializeField] private Button m_info;
        [SerializeField] private Button m_upgrade;
        [SerializeField] private Button m_dismantle;
        [SerializeField] private GameObject[] m_dismantleObjects;

        public override void Show<T>(T context, Action complete = null)
        {
            if (context is SelectBuildingContext model)
            {
                m_name.text = model.BuildingModel.Template.Name.ToUpper();
                m_level.text = $"Lv.{model.BuildingModel.Level}";

                m_upgrade.onClick.AddListener(() => { Node.TriggerEvent(new ShowUpgradeBuildingEventArgs(model.BuildingModel)); });
                m_info.onClick.AddListener(() => { Node.TriggerEvent(new ShowBuildingsInfoEventArgs(model.BuildingModel)); });
                m_dismantle.onClick.AddListener(() => { Node.TriggerEvent(new ShowDismantleBuildingEventArgs(model.BuildingModel)); });

                if (model.BuildingModel.Template.BuildingContext.BuildingType == BuildingType.MainBuilding)
                {
                    foreach (GameObject obj in m_dismantleObjects)
                        obj.SetActive(false);
                }
                else
                {
                    foreach (GameObject obj in m_dismantleObjects)
                        obj.SetActive(true);
                }

                base.Show(context, complete);
                resizer.Recalculate();
            }
        }

        public override void Close(Action complete = null)
        {
            m_info.onClick.RemoveAllListeners();
            m_upgrade.onClick.RemoveAllListeners();
            m_dismantle.onClick.RemoveAllListeners();

            base.Close(complete);
        }
    }
}