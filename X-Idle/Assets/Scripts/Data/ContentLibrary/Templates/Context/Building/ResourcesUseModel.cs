using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class ResourcesUseModel
    {
        [SerializeField] private GameResourceType m_gameResourceType;
        [SerializeField] private float m_amount;

        public GameResourceType GameResourceType => m_gameResourceType;
        public float Amount => m_amount;
    }
}