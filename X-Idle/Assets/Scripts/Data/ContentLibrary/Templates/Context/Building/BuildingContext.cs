using System;
using System.Collections.Generic;
using Data.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingContext
    {
        [SerializeField] private BuildingType m_buildingType;
        [SerializeField] private CostModel m_cost;
        [SerializeField] private bool m_canDuplicate = true;
        [SerializeField] private List<BuildingRequirementModel> m_buildingRequirements;
        [SerializeField] private List<ResourcesRequirementModel> m_resourcesRequirements;
        [SerializeField] private BuildingProductionModel m_buildingProduction;
        [SerializeField] private List<BuildingLevelModelView> m_buildingLevelsView;

        public BuildingType BuildingType => m_buildingType;
        public CostModel Cost => m_cost;
        public bool CanDuplicate => m_canDuplicate;
        public BuildingProductionModel BuildingProduction => m_buildingProduction;
        public List<ResourcesRequirementModel> ResourcesRequirements => m_resourcesRequirements;
        public List<BuildingRequirementModel> BuildingRequirements => m_buildingRequirements;
        public List<BuildingLevelModelView> BuildingLevelsView => m_buildingLevelsView;
    }
}