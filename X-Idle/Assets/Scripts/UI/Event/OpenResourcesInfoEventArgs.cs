using Common.Pattern.BobbleEvent;
using Data.Enum;

namespace UI.Event
{
    public struct OpenResourcesInfoEventArgs  : IEventArgs
    {
        public OpenResourcesInfoEventArgs(GameResourceType resourceType)
        {
            ResourceType = resourceType;
        }

        public GameResourceType ResourceType { get; }
    }
}