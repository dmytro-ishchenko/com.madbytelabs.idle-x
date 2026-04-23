using System;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context
{
    [Serializable]
    public class SceneBuildingContext
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_buildingTemplateId;
        [SerializeField] private Vector3 m_position;
        public string Id => m_id;
        public string BuildingTemplateId => m_buildingTemplateId;
        public Vector3 Position => m_position;

        public SceneBuildingContext(string id, string buildingTemplateId, Vector3 position)
        {
            m_id = id;
            m_buildingTemplateId = buildingTemplateId;
            m_position = position;
        }
    }
}