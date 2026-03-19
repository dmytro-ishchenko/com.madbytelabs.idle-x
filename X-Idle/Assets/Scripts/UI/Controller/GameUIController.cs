using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data.Interface;
using Data.Model;
using UI.Model;
using UI.Popup;
using UnityEngine;

namespace UI.Controller
{
    internal class GameUIController : MonoBehaviour, IGameUIController
    {
        [SerializeField] private PopupManager m_popupManager;

        void Awake()
        {
            Node = new();
        }

        public void ShowSelectBuildingPopup(BuildingModel buildingModel)=> m_popupManager.ShowSelectBuildingPopup(buildingModel);

        public void ShowCreateBuildingPopup(CreateBuildingContext context )=> m_popupManager.ShowCreateBuildingPopup(context);

        public Node Node { get; private set; }
    }
}