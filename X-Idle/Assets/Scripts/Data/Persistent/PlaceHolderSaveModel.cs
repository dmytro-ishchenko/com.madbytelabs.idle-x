using System;
using Data.Enum;
using UnityEngine;

namespace Data.Persistent
{
    [Serializable]
    public class PlaceHolderSaveModel
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_buildingTemplateId;
        [SerializeField] private PlaceHolderType m_placeHolderType;
        [SerializeField] private PlaceHolderStatus m_placeHolderStatus;

        public PlaceHolderSaveModel(string id, string buildingTemplateId, PlaceHolderType placeHolderType, PlaceHolderStatus placeHolderStatus)
        {
            m_id = id;
            m_buildingTemplateId = buildingTemplateId;
            m_placeHolderType = placeHolderType;
            m_placeHolderStatus = placeHolderStatus;
        }

        public string Id => m_id;
        public string BuildingTemplateId => m_buildingTemplateId;
        public PlaceHolderType PlaceHolderType => m_placeHolderType;
        public PlaceHolderStatus PlaceHolderStatus => m_placeHolderStatus;
    }
}