using Data.Enum;
using UnityEngine;

namespace Data.ContentLibrary.Templates.GameResources
{
    public class GameResourcesTemplate : ScriptableObject, IGameResourcesTemplate
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        [SerializeField] GameResourceType m_gameResourceType;
        [SerializeField] private string m_description;
        [SerializeField] private Sprite m_icon;
        [SerializeField] private float m_baseCapacity;
        [SerializeField] private float m_storageFactor;
        public string Id => m_id;
        public string Name => m_name;
        public GameResourceType GameResourceType => m_gameResourceType;
        public string Description => m_description;
        public Sprite Icon => m_icon;
        public float BaseCapacity => m_baseCapacity;
        public float StorageFactor => m_storageFactor;

#if UNITY_EDITOR
        public void SetId(string id)
        {
            m_id = id;
        }
#endif
    }
}