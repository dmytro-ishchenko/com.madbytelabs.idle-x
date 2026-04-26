using System;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class CostModel
    {
        [SerializeField] private float m_scrapCost;
        [SerializeField] private float m_scrapCostGrowth;
        [SerializeField] private float m_partsCost;
        [SerializeField] private float m_partsCostGrowth;

        public float ScrapCost => m_scrapCost;
        public float PartsCost => m_partsCost;
        public float PartsCostGrowth => m_partsCostGrowth;
        public float ScrapCostGrowth => m_scrapCostGrowth;
    }
}