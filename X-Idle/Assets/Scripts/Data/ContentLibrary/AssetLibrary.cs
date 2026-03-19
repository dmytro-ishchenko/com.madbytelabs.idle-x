using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;
using UnityEditor;
using UnityEngine;

namespace Data.ContentLibrary
{
    public class AssetLibrary : ScriptableObject, IAssetLibrary
    {
        [SerializeField] private SerializedDictionary<string, ContentTemplate> m_contentMap;
        [SerializeField] private SerializedDictionary<string, BuildingContext> m_contextMap;
        [SerializeField] private SerializedDictionary<string, GameResourcesTemplate> m_gameResourcesMap;

        public Dictionary<string, ContentTemplate> ContentMap => m_contentMap;
        public Dictionary<string, BuildingContext> ContextMap => m_contextMap;
        public Dictionary<string, GameResourcesTemplate> GameResourcesMap => m_gameResourcesMap;

        public bool TryGetContentTemplate(string id, out ContentTemplate contentTemplate)
        {
            if (m_contentMap.TryGetValue(id, out contentTemplate))
                return true;
            return false;
        }

        public bool TryGetBuildingContext(string id, out BuildingContext buildingContext)
        {
            if (m_contextMap.TryGetValue(id, out buildingContext))
                return true;
            return false;
        }

        public bool TryGetGameResource(string id, out GameResourcesTemplate gameResource)
        {
            if (m_gameResourcesMap.TryGetValue(id, out gameResource))
                return true;
            return false;
        }


#if UNITY_EDITOR
        public void AddContent(string id, ContentTemplate template)
        {
            m_contentMap.TryAdd(id, template);
            EditorUtility.SetDirty(this);
        }

        public void AddContext(string id, BuildingContext context)
        {
            m_contextMap.TryAdd(id, context);
            EditorUtility.SetDirty(this);
        }

        public void AddResource(string newId, GameResourcesTemplate resource)
        {
            m_gameResourcesMap.TryAdd(newId, resource);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}