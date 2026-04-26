using System;
using Data.ContentLibrary.Templates.GameResources;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingProductionModel
    {
        [SerializeField] GameResourcesTemplate m_resourcesTemplate;
        [SerializeField] private float m_amount;
        [SerializeField] private float m_levelMultiplier;
        public IGameResourcesTemplate ResourcesTemplate => m_resourcesTemplate;
        public float Amount => m_amount;
        public float LevelMultiplier => m_levelMultiplier;
    }
}