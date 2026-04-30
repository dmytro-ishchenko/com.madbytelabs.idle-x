using System;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context
{
    [Serializable]
    public class SceneBuildingContext
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_buildingTemplateId;
        [SerializeField] private PlaceHolderType m_placeHolderType;

        public string Id => m_id;
        public string BuildingTemplateId => m_buildingTemplateId;
        public PlaceHolderType PlaceHolderType => m_placeHolderType;

        public SceneBuildingContext(string id, string buildingTemplateId, PlaceHolderType type)
        {
            m_id = id;
            m_buildingTemplateId = buildingTemplateId;
            m_placeHolderType = type;
        }
    }
}