using Common.Pattern.BobbleEvent;

namespace Data.Events
{
    public struct BuildingRequestEventArgs : IEventArgs
    {
        public BuildingRequestEventArgs(string id)
        {
            BuildingId = id;
        }

        public string BuildingId { get; }
    }
}