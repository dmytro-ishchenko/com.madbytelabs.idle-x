using Common.Interface;
using Data.Interface;

namespace GameEnvironment.Factory
{
    public interface IEnvironmentFactory
    {
        IBuildingView GetView(string placeHolderId, string contentId);
    }
}