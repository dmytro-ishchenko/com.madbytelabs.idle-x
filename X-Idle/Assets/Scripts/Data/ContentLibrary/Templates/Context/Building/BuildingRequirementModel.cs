using System;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingRequirementModel
    {
        [SerializeField] private string m_templateId;
        [SerializeField] private int m_level;

        public string TemplateId => m_templateId;
        public int Level => m_level;
    }
}