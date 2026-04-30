using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Events;
using Data.Model;
using Data.Model.Error;
using Data.Model.Popup;

namespace UI.Controller
{
    internal interface IGameUIController : IMonoNode
    {
        void UpdateUserInfo(UserResources resources);
        void SelectPlaceHolder(SelectPlaceHolderEventArgs args, IApplicationData applicationData);
        void ShowUpgradeBuildingPopup(UpgradeBuildingContext context);
        void ShowCreateBuildingErrorPopup(ActionErrorModel model);
        void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData);
        void UpdatePlaceHoldersView(PlaceholderModel model);
    }
}