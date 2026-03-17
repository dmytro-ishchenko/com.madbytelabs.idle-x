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

        public GameObject GetContent(string id)
        {
            if (m_contentMap.TryGetValue(id, out ContentTemplate content))
            {
                return Instantiate(content.View);
            }


            return null;
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