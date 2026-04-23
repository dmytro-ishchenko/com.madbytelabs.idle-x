using AYellowpaper.SerializedCollections;
using Common.Pattern.BobbleEvent;
using Data.Model;
using UI.Enum;
using UI.Model;
using UnityEngine;

namespace UI.Popup
{
    internal class PopupManager : MonoBehaviour, IMonoNode
    {
        [SerializeField] private SerializedDictionary<PopupType, BasePopup> m_popupMap;
        public Node Node { get; } = new();

        void Awake()
        {
            foreach (var popup in m_popupMap.Values)
            {
                Node.AddChild(popup.Node);
            }
        }

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