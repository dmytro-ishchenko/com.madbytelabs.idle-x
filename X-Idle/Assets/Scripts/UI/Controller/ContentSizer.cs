using UnityEngine;
using UnityEngine.UI;

namespace UI.Controller
{
    public class ContentSizer : MonoBehaviour
    {
        [SerializeField] private RectTransform m_root;
        [SerializeField] private bool rebuildAfterResize = true;

        public void Recalculate()
        {
            RecalculateRecursive(m_root);

            if (rebuildAfterResize)
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_root);
        }

        private Vector2 RecalculateRecursive(RectTransform root)
        {
            float totalHeight = 0;
            float totalWidth = 0;
            CustomLayoutElement layoutElementElement = null;

            if (root.childCount > 0)
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    RectTransform child = root.GetChild(i) as RectTransform;

                    if (child == null || !child.gameObject.activeSelf)
                        continue;
                    layoutElementElement = root.GetComponent<CustomLayoutElement>();
                    if (layoutElementElement != null)
                        continue;
                    Vector2 childSize = RecalculateRecursive(child);

                    totalHeight += childSize.y;
                }
            }

            if (layoutElementElement != null)
            {
                switch (layoutElementElement.Axis)
                {
                    case RectTransform.Axis.Horizontal:
                        totalWidth = root.rect.width;
                        if (layoutElementElement.PreferredHeight > 0)
                        {
                            totalHeight = layoutElementElement.PreferredHeight;
                        }
                        else
                        {
                            if (layoutElementElement.MaxHeight > 0 && totalHeight > layoutElementElement.MaxHeight)
                                totalHeight = layoutElementElement.MaxHeight;
                            else if (layoutElementElement.MinHeight > 0 && totalHeight < layoutElementElement.MinHeight)
                                totalHeight = layoutElementElement.MinHeight;
                        }

                        m_root.SetSizeWithCurrentAnchors(layoutElementElement.Axis, totalHeight);
                        break;
                    case RectTransform.Axis.Vertical:
                        totalHeight = root.rect.height;
                        if (layoutElementElement.PreferredWidth > 0)
                        {
                            totalWidth = layoutElementElement.PreferredWidth;
                        }
                        else
                        {
                            if (layoutElementElement.MaxWidth > 0 && totalWidth > layoutElementElement.MaxWidth)
                                totalWidth = layoutElementElement.MaxWidth;
                            else if (layoutElementElement.MinWidth > 0 && totalWidth < layoutElementElement.MinWidth)
                                totalWidth = layoutElementElement.MinWidth;
                        }

                        m_root.SetSizeWithCurrentAnchors(layoutElementElement.Axis, totalHeight);
                        break;
                }

                return new Vector2(totalWidth, totalHeight);
            }

            return Vector2.zero;
        }
    }
}