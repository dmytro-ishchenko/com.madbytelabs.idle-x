using AYellowpaper.SerializedCollections;
using Data.Model;
using UI.Enum;
using UI.Model;
using UnityEngine;

namespace UI.Popup
{
    internal class PopupManager : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<PopupType, BasePopup> m_popupMap;

        public void ShowCreateBuildingPopup(CreateBuildingContext context)
        {
            if (m_popupMap.TryGetValue(PopupType.CreateBuilding, out BasePopup popup))
            {
                popup.Show(context);
            }
        }

        public void ShowSelectBuildingPopup(BuildingModel buildingModel)
        {
            if (m_popupMap.TryGetValue(PopupType.SelectBuilding, out BasePopup popup))
            {
                popup.Show(buildingModel);
            }
        }
    }
}