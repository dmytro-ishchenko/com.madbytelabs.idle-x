using System.Collections.Generic;
using Common.Pattern.BobbleEvent;
using Data.Model;
using UI.Popup;

namespace UI.Controller
{
    internal interface IGameUIController : IMonoNode
    {
        PopupManager PopupManager { get; }
        void UpdateUserInfo(UserResources resources);
        void InitPlaceHoldersView(IReadOnlyDictionary<string, PlaceholderModel> userPlaceHolderData);
        void UpdatePlaceHoldersView(PlaceholderModel model);

        void Pause();
    }
}