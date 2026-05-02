namespace Data.Model.Popup
{
    public struct LockedPlaceHolderContext
    {
        public LockedPlaceHolderContext(string id, PlaceHolderRequirementsModel requirementsModel)
        {
            Id = id;
            RequirementsModel = requirementsModel;
        }

        public string Id { get; }
        public PlaceHolderRequirementsModel RequirementsModel { get; }
    }
}