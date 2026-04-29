using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class UpgradeBuildingRequirementModel
    {
        [SerializeField] private BuildingType m_buildingType;

        [SerializeField] private int m_levelMultiplier;

        public BuildingType BuildingType => m_buildingType;
        public int LevelMultiplier => m_levelMultiplier;
    }
}