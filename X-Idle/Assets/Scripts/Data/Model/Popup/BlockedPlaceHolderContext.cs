namespace Data.Model.Popup
{
    public struct BlockedPlaceHolderContext
    {
        public BlockedPlaceHolderContext(string id, PlaceHolderRequirementsModel requirementsModel)
        {
            Id = id;
            RequirementsModel = requirementsModel;
        }

        public string Id { get; }
        public PlaceHolderRequirementsModel RequirementsModel { get; }
    }
}