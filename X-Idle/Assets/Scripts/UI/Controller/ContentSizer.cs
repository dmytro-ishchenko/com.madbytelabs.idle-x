using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controller
{
    public class ContentSizer : MonoBehaviour
    {
        [SerializeField] private RectTransform m_root;
        [SerializeField] private bool rebuildAfterResize = true;


        public async Awaitable Recalculate()
        {
            RecalculateRecursive(m_root);
            if (rebuildAfterResize)
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_root);
            await Awaitable.NextFrameAsync();
        }

        private Vector2 RecalculateRecursive(RectTransform root)
        {
            float totalHeight = 0;
            float totalWidth = 0;
            CustomLayoutElement layoutElementElement = null;

            layoutElementElement = root.GetComponent<CustomLayoutElement>();
            if (layoutElementElement == null)
                return Vector2.zero;

            if (root.childCount > 0)
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    RectTransform child = root.GetChild(i) as RectTransform;

                    if (child == null || !child.gameObject.activeSelf)
                        continue;
                    layoutElementElement = root.GetComponent<CustomLayoutElement>();
                    if (layoutElementElement == null)
                        continue;
                    Vector2 childSize = RecalculateRecursive(child);

                    totalHeight += childSize.y;
                    totalWidth += childSize.x;
                }
            }

            switch (layoutElementElement.Axis)
            {
                case RectTransform.Axis.Horizontal:
                    if (totalWidth == 0)
                        totalWidth = root.rect.width;

                    if (!layoutElementElement.UseOriginalSize)
                    {
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

                        root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
                    }

                    break;
                case RectTransform.Axis.Vertical:
                    if (totalHeight == 0)
                        totalHeight = root.rect.height;
                    if (!layoutElementElement.UseOriginalSize)
                    {
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

                        root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
                    }

                    break;
            }

            return new Vector2(totalWidth, totalHeight);
        }
    }
}