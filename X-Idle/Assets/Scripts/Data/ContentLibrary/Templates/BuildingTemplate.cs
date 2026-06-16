using Data.ContentLibrary.Templates.Context.Building;
using Data.Interface;
using UnityEditor;
using UnityEngine;

namespace Data.ContentLibrary.Templates
{
    public class BuildingTemplate : ScriptableObject, IBuildingTemplate
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        [SerializeField] private string m_description;
        [SerializeField] private string m_shortDescription;
        [SerializeField] private GameObject m_view;
        [SerializeField] private Sprite m_icon;
        [SerializeField] private Sprite m_iconWide;
        [SerializeField] BuildingContext m_buildingContext;
        public string Id => m_id;

        public string Name => m_name;
        public string Description => m_description;
        public string ShortDescription => m_shortDescription;
        public GameObject View => m_view;
        public Sprite Icon => m_icon;
        public Sprite IconWide => m_iconWide;
        public BuildingContext BuildingContext => m_buildingContext;


#if UNITY_EDITOR
        public void SetId(string id)
        {
            m_id = id;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}