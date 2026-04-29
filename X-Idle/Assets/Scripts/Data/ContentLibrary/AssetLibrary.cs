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
        [SerializeField] private SerializedDictionary<GameResourceType, GameResourcesTemplate> m_gameResourcesMap;

        public IList<BuildingTemplate> Buildings => m_buildingsMap.Values.ToList();

        public bool TryGetBuildingTemplate(string id, out BuildingTemplate buildingTemplate)
        {
            if (m_buildingsMap.TryGetValue(id, out buildingTemplate))
                return true;
            return false;
        }
        

        public bool TryGetGameResource(GameResourceType type, out GameResourcesTemplate gameResource)
        {
            if (m_gameResourcesMap.TryGetValue(type, out gameResource))
                return true;
            return false;
        }


#if UNITY_EDITOR
        public void AddContent(string id, BuildingTemplate template)
        {
            m_buildingsMap.TryAdd(id, template);
            EditorUtility.SetDirty(this);
        }

        public void AddResource(GameResourcesTemplate resource)
        {
            m_gameResourcesMap.TryAdd(resource.GameResourceType, resource);
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