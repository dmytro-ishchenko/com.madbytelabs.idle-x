using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class UpgradeCostModel
    {
        [SerializeField] private List<ResourceCostModel> m_costModels;
        [SerializeField] private int m_upgradeTimeCost;

        public ICollection<ResourceCostModel> CostModels => m_costModels;
        public int UpgradeTimeCost => m_upgradeTimeCost;
    }
}