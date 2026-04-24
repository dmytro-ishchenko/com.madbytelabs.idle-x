using Common.Pattern.BobbleEvent;
using Data.Model;
using UI.Model;
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

        public void ShowSelectBuildingPopup(BuildingModel buildingModel) => m_popupManager.ShowSelectBuildingPopup(buildingModel);

        public void ShowCreateBuildingPopup(CreateBuildingContext context) => m_popupManager.ShowCreateBuildingPopup(context);

        public Node Node { get; private set; }
    }
}