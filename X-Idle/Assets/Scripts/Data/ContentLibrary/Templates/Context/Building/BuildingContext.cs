using System;
using System.Collections.Generic;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingContext
    {
        [SerializeField] private BuildingType m_buildingType;
        [SerializeField] private UpgradeCostModel m_upgradeCost;
        [SerializeField] private DuplicateModel m_duplicateModel;
        [SerializeField] private List<BuildingRequirementModel> m_buildingRequirements;
        [SerializeField] private BuildingProductionModel m_buildingProduction;
        [SerializeField] private List<ResourcesUseModel> m_resourcesRequirements;
        [SerializeField] private List<BuildingLevelModelView> m_buildingLevelsView;

        public BuildingType BuildingType => m_buildingType;
        public UpgradeCostModel UpgradeCostModel => m_upgradeCost;
        public DuplicateModel DuplicateModel => m_duplicateModel;
        public BuildingProductionModel BuildingProduction => m_buildingProduction;
        public List<ResourcesUseModel> ResourcesRequirements => m_resourcesRequirements;
        public List<BuildingRequirementModel> BuildingRequirements => m_buildingRequirements;
        public List<BuildingLevelModelView> BuildingLevelsView => m_buildingLevelsView;
    }
}