using Common.Pattern.BobbleEvent;
using Data.Model;
using UI.Model;

namespace UI.Controller
{
    internal interface IGameUIController:IMonoNode
    {
        void ShowSelectBuildingPopup(BuildingModel buildingModel);
        void ShowCreateBuildingPopup(CreateBuildingContext context);
    }
}