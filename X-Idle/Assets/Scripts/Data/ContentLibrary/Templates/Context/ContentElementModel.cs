using System;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class ContentElementModel
    {
        [SerializeField] private GameObject m_elementView;
        [SerializeField] Vector3 m_position;
        [SerializeField] Vector3 m_rotation;
        public GameObject ElementView => m_elementView;
        public Vector3 Position => m_position;
        public Vector3 Rotation => m_rotation;
    }
}