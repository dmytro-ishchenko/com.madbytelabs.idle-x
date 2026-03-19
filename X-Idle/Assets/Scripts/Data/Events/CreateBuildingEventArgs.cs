using Common.Pattern.BobbleEvent;

namespace Data.Events
{
    public struct CreateBuildingEventArgs: IEventArgs
    {
        public CreateBuildingEventArgs(string placeHolderId)
        {
            PlaceHolderId = placeHolderId;
        }

        public string PlaceHolderId { get; }
    }
}