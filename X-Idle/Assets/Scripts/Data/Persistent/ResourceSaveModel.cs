using System;
using Data.Enum;
using UnityEngine;

namespace Data.Persistent
{
    [Serializable]
    public class ResourceSaveModel
    {
        [SerializeField] private GameResourceType m_resourceType;
        [SerializeField] private float m_amount;

        public ResourceSaveModel(GameResourceType resourceType, float amount)
        {
            m_resourceType = resourceType;
            m_amount = amount;
        }

        public GameResourceType ResourceType => m_resourceType;
        public float Amount => m_amount;
    }
}