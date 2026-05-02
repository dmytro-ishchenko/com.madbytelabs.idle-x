using Common.Pattern.BobbleEvent;
using Data.Enum;

namespace Data.Events
{
    public struct SelectPlaceHolderEventArgs: IEventArgs
    {
        public SelectPlaceHolderEventArgs(string placeHolderId, PlaceHolderStatus placeHolderStatus)
        {
            PlaceHolderId = placeHolderId;
            PlaceHolderStatus = placeHolderStatus;
        }

        public string PlaceHolderId { get; }
        public PlaceHolderStatus PlaceHolderStatus { get; }
    }
}