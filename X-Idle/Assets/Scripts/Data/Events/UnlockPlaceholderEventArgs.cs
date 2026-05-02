using Common.Pattern.BobbleEvent;

namespace Data.Events
{
    public struct UnlockPlaceholderEventArgs: IEventArgs
    {
        public UnlockPlaceholderEventArgs(string placeHolderId)
        {
            PlaceHolderId = placeHolderId;
        }

        public string PlaceHolderId { get; }
    }
}