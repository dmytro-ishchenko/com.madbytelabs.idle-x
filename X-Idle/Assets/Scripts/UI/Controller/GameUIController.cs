using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data.Model;
using UI.Enum;
using UI.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controller
{
    internal class GameUIController : MonoBehaviour, IGameUIController
    {
        [SerializeField] private PopupManager m_popupManager;
        [SerializeField] private UserInfoViewController m_userInfo;
        [SerializeField] private PlaceholderViewController m_placeholderView;
        [SerializeField] private Button m_settingsButton;
        public Node Node { get; private set; }

        public PopupManager PopupManager => m_popupManager;

        void Awake()
        {
            Node = new();
            Node.AddChild(m_popupManager.Node);
            Node.AddChild(m_placeholderView.Node);
            Node.AddChild(m_userInfo.Node);
            m_settingsButton.onClick.AddListener(() => { m_popupManager.ShowPopup(PopupType.Settings); });
        }

        public void UpdateUserInfo(UserResources resources) => m_userInfo.UpdateUserResources(resources);
        public void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData) => m_placeholderView.InitPlaceHoldersView(userPlaceHolderData);
        public void UpdatePlaceHoldersView(PlaceholderModel model) => m_placeholderView.UpdatePlaceHoldersView(model);
    }
}