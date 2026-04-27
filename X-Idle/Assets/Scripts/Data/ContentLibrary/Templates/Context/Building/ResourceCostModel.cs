using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class ResourceCostModel
    {
        [SerializeField] private GameResourceType m_gameResourceType;
        [SerializeField] private float m_cost;
        [SerializeField] private float m_costGrowth;

        public GameResourceType GameResourceType => m_gameResourceType;
        public float Cost => m_cost;
        public float CostGrowth => m_costGrowth;
    }
}