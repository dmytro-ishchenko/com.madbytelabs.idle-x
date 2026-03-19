using System.Collections.Generic;

namespace Data.Model
{
    public class UserBuildingsData
    {
        private Dictionary<string, BuildingModel> m_buildingsMap = new();
        public IDictionary<string, BuildingModel> BuildingsMap => m_buildingsMap;

        public bool TryGetBuildingModel(string name, out BuildingModel buildingModel)
        {
            return m_buildingsMap.TryGetValue(name, out buildingModel);
        }

        public void AddBuildingModel(string id, BuildingModel buildingModel)
        {
            m_buildingsMap.TryAdd(id, buildingModel);
        }
    }
}