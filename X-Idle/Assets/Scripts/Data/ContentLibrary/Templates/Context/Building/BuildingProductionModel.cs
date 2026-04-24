using System;
using Data.ContentLibrary.Templates.GameResources;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingProductionModel
    {
        [SerializeField] GameResourcesTemplate m_resourcesTemplate;
        [SerializeField] private int m_amountPerSecond;
        public IGameResourcesTemplate ResourcesTemplate => m_resourcesTemplate;
        public int AmountPerSecond => m_amountPerSecond;
    }
}