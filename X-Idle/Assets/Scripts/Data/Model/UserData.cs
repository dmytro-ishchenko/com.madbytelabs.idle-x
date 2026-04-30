using System.Collections.Generic;

namespace Data.Model
{
    internal class UserData
    {
        public UserData(UserPlaceHolderData placeHolderData, List<BuildingModel> buildings)
        {
            UserPlaceHolderData = placeHolderData;

            if (buildings is { Count: > 0 })
            {
                foreach (var building in buildings)
                {
                    UserBuildingsData.AddStartBuildingModel(building.Id, building);
                }
            }
        }

        public UserBuildingsData UserBuildingsData { get; } = new();
        public UserResources UserResources { get; } = new();
        public UserPlaceHolderData UserPlaceHolderData { get; }
    }
}