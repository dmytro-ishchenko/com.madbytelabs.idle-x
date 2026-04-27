using Common.Pattern.BobbleEvent;
using Data.Model;
using UI.Model;

namespace UI.Controller
{
    internal interface IGameUIController : IMonoNode
    {
        void UpdateUserInfo(UserResources resources);
        void ShowUpgradeBuildingPopup(UpgradeBuildingContext context);
        void ShowCreateBuildingPopup(CreateBuildingContext context);
        void ShowCreateBuildingErrorPopup(CreateBuildingErrorContext context);
    }
}