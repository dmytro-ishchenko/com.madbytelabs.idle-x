using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingCostModel
    {
        [SerializeField] private List<ResourceCostModel> m_costModels;
        [SerializeField] private int m_timeCost;

        public ICollection<ResourceCostModel> CostModels => m_costModels;
        public int TimeCost => m_timeCost;
    }
}