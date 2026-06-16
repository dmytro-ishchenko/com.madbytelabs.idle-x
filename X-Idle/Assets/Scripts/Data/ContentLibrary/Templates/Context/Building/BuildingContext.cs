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
        [SerializeField] private int m_maxLevel;
        [SerializeField] private DuplicateModel m_duplicateModel;
        [SerializeField] private List<CreateBuildingRequirementModel> m_createBuildingRequirements;
        [SerializeField] private BuildingCostModel m_createCost;
        [SerializeField] private List<UpgradeBuildingRequirementModel> m_upgradeBuildingRequirements;
        [SerializeField] private BuildingCostModel m_upgradeCost;
        [SerializeField] private BuildingProductionModel m_buildingProduction;
        [SerializeField] private List<ResourcesUseModel> m_resourcesUse;
        [SerializeField] private List<BuildingLevelModelView> m_buildingLevelsView;

        public BuildingType BuildingType => m_buildingType;
        public int MaxLevel => m_maxLevel;
        public DuplicateModel DuplicateModel => m_duplicateModel;
        public BuildingProductionModel BuildingProduction => m_buildingProduction;
        public ICollection<ResourcesUseModel> ResourcesUse => m_resourcesUse;
        public ICollection<CreateBuildingRequirementModel> CreateBuildingRequirements => m_createBuildingRequirements;
        public BuildingCostModel CreateCost => m_createCost;
        public ICollection<UpgradeBuildingRequirementModel> UpgradeBuildingRequirements => m_upgradeBuildingRequirements;
        public BuildingCostModel UpgradeCost => m_upgradeCost;
        public ICollection<BuildingLevelModelView> BuildingLevelsView => m_buildingLevelsView;
    }
}