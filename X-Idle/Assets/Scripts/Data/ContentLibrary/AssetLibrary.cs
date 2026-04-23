using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.GameResources;
using Data.Enum;
using UnityEditor;
using UnityEngine;

namespace Data.ContentLibrary
{
    public class AssetLibrary : ScriptableObject, IAssetLibrary
    {
        [SerializeField] private SerializedDictionary<string, BuildingTemplate> m_buildingsMap;
        [SerializeField] private SerializedDictionary<string, GameResourcesTemplate> m_gameResourcesMap;

        public IList<BuildingTemplate> Buildings => m_buildingsMap.Values.ToList();

        public bool TryGetBuildingTemplate(string id, out BuildingTemplate buildingTemplate)
        {
            if (m_buildingsMap.TryGetValue(id, out buildingTemplate))
                return true;
            return false;
        }

        public bool TryGetBuildingTemplate(BuildingType type, out BuildingTemplate buildingTemplate)
        {
            var template = m_buildingsMap.Values.ToList().First();

            if (template != null)
            {
                buildingTemplate = template;
                return true;
            }

            buildingTemplate = null;
            return false;
        }

        public bool TryGetGameResource(string id, out GameResourcesTemplate gameResource)
        {
            if (m_gameResourcesMap.TryGetValue(id, out gameResource))
                return true;
            return false;
        }


#if UNITY_EDITOR
        public void AddContent(string id, BuildingTemplate template)
        {
            m_buildingsMap.TryAdd(id, template);
            EditorUtility.SetDirty(this);
        }

        public void AddResource(string newId, GameResourcesTemplate resource)
        {
            m_gameResourcesMap.TryAdd(newId, resource);
            EditorUtility.SetDirty(this);
        }

        public void ClearLibrary()
        {
            m_buildingsMap.Clear();
            m_gameResourcesMap.Clear();
        }
#endif
    }
}