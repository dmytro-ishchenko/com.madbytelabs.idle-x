using System.Collections.Generic;
using UI.Model;
using UI.Popup.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    internal class CreateBuildingPopup : BasePopup
    {
        [SerializeField] private BuildingElement m_source;
        [SerializeField] private Transform m_root;
        [SerializeField] private ScrollRect m_scrollRect;
        [SerializeField] Button m_createButton;
        private List<BuildingElement> m_elements = new();

        public override void Show<T>(T context)
        {
            m_scrollRect.verticalNormalizedPosition = 1.0f;
            if (context is CreateBuildingContext model)
            {
                foreach (var buildingTemplate in model.Buildings)
                {
                    var element = Instantiate(m_source);
                    element.transform.SetParent(m_root);
                    element.Init(buildingTemplate);
                    element.gameObject.SetActive(true);
                    m_elements.Add(element);
                }

                base.Show(context);
            }
        }

        public override void Close()
        {
            foreach (var element in m_elements)
            {
                Destroy(element.gameObject);
            }

            m_elements.Clear();
            base.Close();
        }
    }
}