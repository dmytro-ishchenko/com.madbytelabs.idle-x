using System;
using System.Collections.Generic;

namespace Data.Model
{
    internal class UserData
    {
        public UserData(List<BuildingModel> buildings)
        {
            UserBuildingsData = new UserBuildingsData();

            if (buildings != null && buildings.Count > 0)
            {
                foreach (var building in buildings)
                {
                    UserBuildingsData.BuildingsMap.Add(building.Id, building);
                }
            }
        }

        public event Action<UserResources> OnUserResourcesChanged;


        public UserResources UserResources { get; private set; } = new();
        public UserBuildingsData UserBuildingsData { get; }
    

        
    }
}