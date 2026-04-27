using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingRequirementModel
    {
        [SerializeField] private BuildingType m_buildingType;
        [SerializeField] private int m_level;

        public BuildingType BuildingType => m_buildingType;
        public int Level => m_level;
    }
}