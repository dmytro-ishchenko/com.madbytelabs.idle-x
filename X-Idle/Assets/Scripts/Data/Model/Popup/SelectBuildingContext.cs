namespace Data.Model.Popup
{
    public class SelectBuildingContext : IShowPopupContext
    {
        public SelectBuildingContext(string id, BuildingModel buildingModel)
        {
            Id = id;
            BuildingModel = buildingModel;
        }

        public string Id { get; }

        public BuildingModel BuildingModel { get; }
    }
}