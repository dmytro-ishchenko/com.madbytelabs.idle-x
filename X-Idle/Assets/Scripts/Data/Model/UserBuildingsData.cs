using System.Collections.Generic;

namespace Data.Model
{
    public class UserBuildingsData
    {
        private Dictionary<string, BuildingModel> m_buildingsMap = new();
        public IDictionary<string, BuildingModel> BuildingsMap => m_buildingsMap;

        public bool TryGetBuildingModel(string id, out BuildingModel buildingModel)
        {
            return m_buildingsMap.TryGetValue(id, out buildingModel);
        }

        public void AddBuildingModel(string id, BuildingModel buildingModel)
        {
            m_buildingsMap.TryAdd(id, buildingModel);
        }
    }
}