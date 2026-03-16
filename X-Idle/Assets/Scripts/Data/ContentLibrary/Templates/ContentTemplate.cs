using Data.ContentLibrary.Templates.Context;
using UnityEditor;
using UnityEngine;

namespace Data.ContentLibrary.Templates
{
    public class ContentTemplate : ScriptableObject, IContentTemplate
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        [SerializeField] private string m_description;
        [SerializeField] private GameObject m_view;
        [SerializeField] ContentContext m_contentContext;
        public string Id => m_id;

        public string Name => m_name;
        public string Description => m_description;
        public GameObject View => m_view;

        public T GetContentContext<T>() where T : ContentContext
        {
            if (m_contentContext != null)
                return (T)m_contentContext;

            return null;
        }


#if UNITY_EDITOR
        public void SetId(string id)
        {
            m_id = id;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}