using System;
using System.Collections.Generic;
using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Placeholder
{
    [Serializable]
    public class PlaceholderStatusRequirementModel
    {
        [SerializeField] private PlaceHolderType m_placeholderType;
        [SerializeField] private List<RequiredBuildingModel> m_requiredBuildings = new();
        [SerializeField] private List<RequiredResourceModel> m_rquiredResources = new();

        public PlaceHolderType PlaceHolderType => m_placeholderType;
        public List<RequiredBuildingModel> RequiredBuildings => m_requiredBuildings;
        public List<RequiredResourceModel> RequiredResources => m_rquiredResources;
    }
}