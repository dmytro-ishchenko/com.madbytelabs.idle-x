using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingLevelModel
    {
        [SerializeField] private int m_level;
        [SerializeField] private string m_description;
        [SerializeField] LevelRequirementsModel m_levelRequirements;
        [SerializeField] private GameObject m_mainView;
        [SerializeField] List<ContentElementModel> m_elements;
        public int Level => m_level;
        public string Description => m_description;
        public LevelRequirementsModel LevelRequirements => m_levelRequirements;
        public GameObject MainView => m_mainView;
        public List<ContentElementModel> Elements => m_elements;
    }
}