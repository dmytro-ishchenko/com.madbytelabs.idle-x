using System;
using UnityEngine;

namespace Data.Persistent
{
    [Serializable]
    public class BuildingSaveModel
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_buildingTemplateId;
        [SerializeField] private int m_level;

        public BuildingSaveModel(string id, string buildingTemplateId, int level)
        {
            m_id = id;
            m_buildingTemplateId = buildingTemplateId;
            m_level = level;
        }

        public string Id => m_id;
        public string BuildingTemplateId => m_buildingTemplateId;
        public int Level => m_level;
#if UNITY_EDITOR
        public void SetBuildingId(string buildingTemplateId)
        {
            m_buildingTemplateId = buildingTemplateId;
        }

        public void SetLevel(int level)
        {
            m_level = level;
        }
#endif
    }
}