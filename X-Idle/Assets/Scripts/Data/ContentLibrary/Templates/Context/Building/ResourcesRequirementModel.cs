using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class ResourcesRequirementModel
    {
        [SerializeField] private ResourcesType m_resourceType;
        [SerializeField]  private float m_use;

        public ResourcesType ResourceType => m_resourceType;
        public float Use => m_use;
    }
}