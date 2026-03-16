using System;
using UnityEditor;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context
{
    [Serializable]
    public class ContentContext : ScriptableObject, IContentContext
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        [SerializeField] private string m_description;
        public string Id => m_id;
        public string Name => m_name;
        public string Description => m_description;


#if UNITY_EDITOR
        public void SetId(string id)
        {
            m_id = id;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}