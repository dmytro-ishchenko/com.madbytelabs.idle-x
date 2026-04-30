using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Placeholder
{
    [Serializable]
    public class RequiredResourceModel
    {
        [SerializeField] private GameResourceType m_resourceType;
        [SerializeField] private int m_requiredAmount;
        public GameResourceType ResourceType => m_resourceType;
        public float RequireAmount => m_requiredAmount;
    }
}