using Common.Pattern.BobbleEvent;
using Data.Enum;
using Data.Model;

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