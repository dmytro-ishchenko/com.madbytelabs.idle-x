using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameEnvironment.Controller
{
    public class PlaceholderRoot : MonoBehaviour
    {
        [SerializeField] List<PlaceHolder> m_placeHolders;
        public IList<PlaceHolder> PlaceHolders => m_placeHolders;

#if UNITY_EDITOR
        public void AddPlaceholder(PlaceHolder placeHolder)
        {
            m_placeHolders.Add(placeHolder);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}