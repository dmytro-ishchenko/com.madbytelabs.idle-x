using Common.Pattern.BobbleEvent;
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

        void Awake()
        {
            Node = new();
            Node.AddChild(m_popupManager.Node);
        }

        public void UpdateUserInfo(UserResources resources) => m_userInfo.UpdateUserResources(resources);

        public void ShowCreateBuildingPopup(CreateBuildingContext context) => m_popupManager.ShowCreateBuildingPopup(context);
        
        public void ShowCreateBuildingErrorPopup(ActionErrorModel model)=> m_popupManager.ShowCreateBuildingErrorPopup(model);
        
        public void ShowUpgradeBuildingPopup(UpgradeBuildingContext context) => m_popupManager.ShowUpgradeBuildingPopup(context);

        public Node Node { get; private set; }
    }
}