using System;
using AYellowpaper.SerializedCollections;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Enum;
using Data.Events;
using Data.Model.Popup;
using UI.Enum;
using UI.Event;
using UnityEngine;

namespace UI.Popup
{
    internal class PopupManager : MonoBehaviour, IMonoNode
    {
        [SerializeField] private SerializedDictionary<PopupType, BasePopup> m_popupMap;
        private BasePopup m_openedPopup;
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
                    ShowPopup(PopupType.CreateBuilding, new CreateBuildingContext(args.PlaceHolderId, applicationData.GetAvailableBuilding(), applicationData));
                    break;
                case PlaceHolderStatus.Locked:
                    ShowPopup(PopupType.LockedPlaceHolder, new LockedPlaceHolderContext(args.PlaceHolderId, applicationData.GetPlaceHolderRequirements(args.PlaceHolderId)));
                    break;
                case PlaceHolderStatus.Blocked:
                    ShowPopup(PopupType.BlockedPlaceHolder, new LockedPlaceHolderContext(args.PlaceHolderId, applicationData.GetPlaceHolderRequirements(args.PlaceHolderId)));
                    break;
            }
        }

        public void ShowPopup(PopupType popupType)
        {
            if (m_popupMap.TryGetValue(popupType, out BasePopup popup))
            {
                if (m_openedPopup != null)
                {
                    m_openedPopup.Close(() =>
                    {
                        m_openedPopup = popup;
                        m_openedPopup.Show();
                    });
                }
                else
                {
                    m_openedPopup = popup;
                    m_openedPopup.Show();
                }
            }
        }

        public void ShowPopup<T>(PopupType popupType, T args)
        {
            if (m_popupMap.TryGetValue(popupType, out BasePopup popup))
            {
                if (m_openedPopup != null)
                {
                    if (m_openedPopup != popup && m_openedPopup.IsActive)
                    {
                        m_openedPopup.Close(() =>
                        {
                            m_openedPopup = popup;
                            m_openedPopup.Show(args);
                        });
                    }
                    else
                    {
                        m_openedPopup = popup;
                        m_openedPopup.Show(args);
                    }
                }
                else
                {
                    m_openedPopup = popup;
                    m_openedPopup.Show(args);
                }
            }
        }

        public void ShowCurtains()
        {
            ShowPopup(PopupType.Curtains, new ShowCurtainsEventArgs());
        }

        public void ShowCurtainImmediately(float time, Action<CurtainsState> onStateChanged)
        {
            ShowPopup(PopupType.Curtains, new ShowCurtainsEventArgs(time, true, onStateChanged));
        }

        public void ShowCurtainsOnTime(float time, Action<CurtainsState> onStateChanged)
        {
            ShowPopup(PopupType.Curtains, new ShowCurtainsEventArgs(time, false, onStateChanged));
        }

        public void CloseAllPopups()
        {
            if (m_openedPopup != null && m_openedPopup.IsActive)
                m_openedPopup.CloseImmediately();
        }
    }
}