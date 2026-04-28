using System;
using Data.ContentLibrary.Templates.GameResources;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class ResourcesUseModel
    {
        [SerializeField] private GameResourcesTemplate m_gameResource;
        [SerializeField] private float m_amount;

        public GameResourcesTemplate GameResource => m_gameResource;
        public float Amount => m_amount;
    }
}