using UnityEngine;

namespace UI.Controller
{
    public class CustomLayoutElement : MonoBehaviour
    {
        [SerializeField] RectTransform.Axis m_axis;
        [SerializeField] private float m_minHeight;
        [SerializeField] private float m_minWidth;
        [SerializeField] private float m_preferredHeight;
        [SerializeField] private float m_preferredWidth;
        [SerializeField] private float m_maxHeight;
        [SerializeField] private float m_maxWidth;
        [SerializeField] private bool m_useOriginalSize;
        public RectTransform.Axis Axis => m_axis;
        public float MinHeight => m_minHeight;
        public float MinWidth => m_minWidth;
        public float PreferredHeight => m_preferredHeight;
        public float PreferredWidth => m_preferredWidth;
        public float MaxHeight => m_maxHeight;
        public float MaxWidth => m_maxWidth;
        public bool UseOriginalSize => m_useOriginalSize;
    }
}