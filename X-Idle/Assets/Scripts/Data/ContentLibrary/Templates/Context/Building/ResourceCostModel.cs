using System;
using Data.ContentLibrary.Templates.GameResources;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class ResourceCostModel
    {
        [SerializeField] private GameResourcesTemplate m_gameResourceTemplate;
        [SerializeField] private float m_cost;
        [SerializeField] private float m_costGrowth;

        public GameResourcesTemplate GameResource => m_gameResourceTemplate;
        public float Cost => m_cost;
        public float CostGrowth => m_costGrowth;
    }
}