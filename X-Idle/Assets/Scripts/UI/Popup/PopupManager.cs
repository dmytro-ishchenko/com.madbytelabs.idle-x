using AYellowpaper.SerializedCollections;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Enum;
using Data.Events;
using Data.Model.Error;
using Data.Model.Popup;
using UI.Enum;
using UI.Event;
using UnityEngine;

namespace UI.Popup
{
    internal class PopupManager : MonoBehaviour, IMonoNode
    {
        [SerializeField] private SerializedDictionary<PopupType, BasePopup> m_popupMap;
        BasePopup m_openedPopup;
        public Node Node { get; } = new();

        void Awake()
        {
            foreach (var popup in m_popupMap.Values)
            {
                Node.AddChild(popup.Node);
            }
        }

        public void SelectPlaceHolder(SelectPlaceHolderEventArgs args, IApplicationData applicationData)
        {
            switch (args.PlaceHolderStatus)
            {
                case PlaceHolderStatus.Unlocked:
                    ShowPopup(PopupType.CreateBuilding, new CreateBuildingContext(args.PlaceHolderId, applicationData.GetAvailableBuilding()));
                    break;
                case PlaceHolderStatus.Locked:
                    ShowPopup(PopupType.LockedPlaceHolder, new LockedPlaceHolderContext(args.PlaceHolderId, applicationData.GetPlaceHolderRequirements(args.PlaceHolderId)));
                    break;
                case PlaceHolderStatus.Blocked:
                    ShowPopup(PopupType.BlockedPlaceHolder, new LockedPlaceHolderContext(args.PlaceHolderId, applicationData.GetPlaceHolderRequirements(args.PlaceHolderId)));
                    break;
            }
        }

        public void ShowPopup<T>(PopupType popupType, T args)
        {
            if (m_popupMap.TryGetValue(popupType, out BasePopup popup))
            {
                if (m_openedPopup != null)
                    m_openedPopup.Close();
                m_openedPopup = popup;
                m_openedPopup.Show(args);
            }
        }
    }
}