using Common.Interface;
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

        public IContentContext GetContentContext<T>() where T : IContentContext
        {
            if (m_contentContext != null)
            {
                if (m_contentContext is T context)
                {
                    return context;
                }
            }

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