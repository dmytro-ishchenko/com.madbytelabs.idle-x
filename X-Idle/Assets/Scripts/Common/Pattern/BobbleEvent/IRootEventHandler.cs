namespace Common.Pattern.BobbleEvent
{
    public interface IRootEventHandler
    {
        void Handle(NodeEvent evt);
    }

    public struct NodeEvent
    {
        public string EventName { get; }
        public IEventArgs EventArgs { get; }

        public NodeEvent(string eventName, IEventArgs eventArgs)
        {
            EventName = eventName;
            EventArgs = eventArgs;
        }
    }
}