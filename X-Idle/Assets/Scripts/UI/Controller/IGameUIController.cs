using Common.Model;
using Common.Pattern.BobbleEvent;

namespace UI.Controller
{
    internal interface IGameUIController:IMonoNode
    {
        void ShowSelectContentPopup(ContentModel contentModel);
    }
}