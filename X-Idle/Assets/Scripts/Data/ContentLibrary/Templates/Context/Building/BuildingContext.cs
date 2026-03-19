using System;
using System.Collections.Generic;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingContext : ContentContext
    {
        [SerializeField] private BuildingType m_buildingType;
        [SerializeField] private bool m_canDuplicate = true;
        [SerializeField] private BuildingProductionModel m_buildingProduction;
        [SerializeField] private List<BuildingLevelModel> m_buildingLevels;

        public BuildingType BuildingType => m_buildingType;
        public bool CanDuplicate => m_canDuplicate;
        public BuildingProductionModel BuildingProduction => m_buildingProduction;
        public List<BuildingLevelModel> BuildingLevels => m_buildingLevels;
    }
}