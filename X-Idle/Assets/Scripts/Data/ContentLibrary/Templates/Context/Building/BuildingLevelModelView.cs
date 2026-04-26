using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class BuildingLevelModelView
    {
        [SerializeField] private int m_level;
        [SerializeField] private string m_description;
        [SerializeField] private GameObject m_mainView;
        [SerializeField] List<ContentElementModel> m_elements;
        public int Level => m_level;
        public string Description => m_description;
        public GameObject MainView => m_mainView;
        public List<ContentElementModel> Elements => m_elements;
    }
}