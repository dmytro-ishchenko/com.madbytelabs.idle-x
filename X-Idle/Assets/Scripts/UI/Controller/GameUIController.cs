using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Events;
using Data.Model;
using Data.Model.Error;
using Data.Model.Popup;
using UI.Popup;
using UnityEngine;

namespace UI.Controller
{
    internal class GameUIController : MonoBehaviour, IGameUIController
    {
        [SerializeField] private PopupManager m_popupManager;
        [SerializeField] private UserInfoViewController m_userInfo;
        [SerializeField] private PlaceholderViewController m_placeholderView;
        public Node Node { get; private set; }

        public PopupManager PopupManager => m_popupManager;

        void Awake()
        {
            Node = new();
            Node.AddChild(m_popupManager.Node);
            Node.AddChild(m_placeholderView.Node);
        }

        public void UpdateUserInfo(UserResources resources) => m_userInfo.UpdateUserResources(resources);
        public void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData) => m_placeholderView.InitPlaceHoldersView(userPlaceHolderData);
        public void UpdatePlaceHoldersView(PlaceholderModel model) => m_placeholderView.UpdatePlaceHoldersView(model);
    }
}