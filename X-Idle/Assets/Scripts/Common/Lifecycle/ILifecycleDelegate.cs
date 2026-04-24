namespace Common.Lifecycle
{
    public interface ILifecycleDelegate
    {
        void OnApplicationQuit();
        void OnApplicationFocus(bool hasFocus);
        void OnApplicationPause(bool pauseStatus);
    }
}