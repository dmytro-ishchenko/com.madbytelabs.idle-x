using Common.Interface;

namespace GameEnvironment.Factory
{
    public interface IEnvironmentFactory
    {
        IContentView GetView(string contentId);
    }
}