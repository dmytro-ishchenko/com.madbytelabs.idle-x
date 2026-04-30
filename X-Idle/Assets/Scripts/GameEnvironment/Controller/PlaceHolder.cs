using System;
using Data.Enum;
using Data.Interface;
using UnityEditor;
using UnityEngine;

namespace GameEnvironment.Controller
{
    public class PlaceHolder : MonoBehaviour
    {
        [SerializeField] private string m_id;
        [SerializeField] private string m_contentId;
        [SerializeField] private PlaceHolderType m_placeholderType;
        public string Id => m_id;
        public string ContentId => m_contentId;
        public PlaceHolderType Type => m_placeholderType;

        public IBuildingView View { get; private set; }

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

        public void SetType(PlaceHolderType type)
        {
            m_placeholderType = type;
            EditorUtility.SetDirty(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.localPosition, Vector3.one * 5f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.localPosition, Vector3.one * 5f);
        }
#endif
        public void AddContent(IBuildingView view)
        {
            view.GameObject.transform.SetParent(transform);
            view.GameObject.transform.localPosition = Vector3.zero;
            view.GameObject.transform.localEulerAngles = Vector3.zero;

            View = view;
        }

        public void RemoveContent()
        {
            Destroy(View.GameObject);
        }
    }
}