using UnityEditor;
using UnityEngine;

namespace GameEnvironment.Controller
{
    public class PlaceHolder : MonoBehaviour
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_contentId;

        public string Id => m_id;
        public string ContentId => m_contentId;

#if UNITY_EDITOR
        public void SetId(string id)
        {
            m_id = id;
            EditorUtility.SetDirty(this);
        }

        public void SetContentId(string id)
        {
            m_contentId = id;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}