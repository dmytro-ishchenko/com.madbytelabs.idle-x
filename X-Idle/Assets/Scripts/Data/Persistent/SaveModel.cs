using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Persistent
{
    [Serializable]
    public class SaveModel
    {
        [SerializeField] private List<PlaceHolderSaveModel> m_placeholders;
        [SerializeField] private List<BuildingSaveModel> m_buildings;
        [SerializeField] private List<ResourceSaveModel> m_resources;

        public SaveModel(List<PlaceHolderSaveModel> placeholders, List<BuildingSaveModel> buildings, List<ResourceSaveModel> resources)
        {
            DateTime now = DateTime.UtcNow;
            Time = ((DateTimeOffset)now).ToUnixTimeSeconds();
            m_placeholders = placeholders;
            m_buildings = buildings;
            m_resources = resources;
        }

        public long Time { get; }
        public List<PlaceHolderSaveModel> PlaceHolders => m_placeholders;
        public List<BuildingSaveModel> Buildings => m_buildings;
        public List<ResourceSaveModel> Resources => m_resources;
    }
}