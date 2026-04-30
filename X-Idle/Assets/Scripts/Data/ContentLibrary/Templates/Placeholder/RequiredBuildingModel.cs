using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Placeholder
{
    [Serializable]
    public class RequiredBuildingModel
    {
        [SerializeField] private BuildingType m_buildingType;
        [SerializeField] private int m_requiredLevel;
        public BuildingType BuildingType => m_buildingType;
        public int RequireLevel => m_requiredLevel;
    }
}