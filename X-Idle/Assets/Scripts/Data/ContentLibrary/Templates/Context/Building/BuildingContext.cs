using System.Collections.Generic;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    public class BuildingContext : ContentContext
    {
        [SerializeField] BuildingProductionModel m_buildingProduction;
        [SerializeField] List<BuildingLevelModel> m_buildingLevels;
        public BuildingProductionModel BuildingProduction => m_buildingProduction;
        public List<BuildingLevelModel> BuildingLevels => m_buildingLevels;
    }
}