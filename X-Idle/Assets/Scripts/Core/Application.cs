using Core.Bootstrapper;

namespace Core
{
    public class Application
    {
        IBootstrapper m_bootstrapper;
        
        public Application(IBootstrapper bootstrapper)
        {
            bootstrapper.Bootstrap();
        }
    }
}