using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data;
using Data.Events;
using Data.Model;
using Data.Model.Error;
using Data.Model.Popup;
using UI.Popup;

namespace UI.Controller
{
    internal interface IGameUIController : IMonoNode
    {
        PopupManager PopupManager { get; }
        void UpdateUserInfo(UserResources resources);
        void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData);
        void UpdatePlaceHoldersView(PlaceholderModel model);
    }
}