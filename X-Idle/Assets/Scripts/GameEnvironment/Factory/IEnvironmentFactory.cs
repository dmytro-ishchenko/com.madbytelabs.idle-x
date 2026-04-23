using Common.Interface;
using Data.ContentLibrary.Templates;
using Data.Interface;

namespace GameEnvironment.Factory
{
    public interface IEnvironmentFactory
    {
        IBuildingView GetView(string placeHolderId, IBuildingTemplate contentTemplate);
    }
}