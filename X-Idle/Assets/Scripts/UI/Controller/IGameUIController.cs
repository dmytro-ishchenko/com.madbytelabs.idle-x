using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data.Model;
using Data.Model.Error;
using Data.Model.Popup;

namespace UI.Controller
{
    internal interface IGameUIController : IMonoNode
    {
        void UpdateUserInfo(UserResources resources);
        void ShowUpgradeBuildingPopup(UpgradeBuildingContext context);
        void ShowCreateBuildingPopup(CreateBuildingContext context);
        void ShowCreateBuildingErrorPopup(ActionErrorModel model);
        void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData);
    }
}