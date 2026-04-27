using Data.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.ContentLibrary.Templates.GameResources
{
    public class GameResourcesTemplate : ScriptableObject, IGameResourcesTemplate
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        [FormerlySerializedAs("m_resourcesType")] [SerializeField] GameResourceType mGameResourceType;
        [SerializeField] private string m_description;
        [SerializeField] private Sprite m_icon;
        [SerializeField] private float m_baseCapacity;
        public string Id => m_id;
        public string Name => m_name;
        public GameResourceType GameResourceType => mGameResourceType;
        public string Description => m_description;
        public Sprite Icon => m_icon;
        public float BaseCapacity => m_baseCapacity;

#if UNITY_EDITOR
        public void SetId(string id)
        {
            m_id = id;
        }
#endif
    }
}