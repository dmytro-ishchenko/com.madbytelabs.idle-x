using System.Collections.Generic;
using Data.ContentLibrary.Templates;
using UnityEngine;

namespace Tool.Popup
{
    public class AddContentToolPopup : BaseToolPopup
    {
        public AddContentToolPopup()
        {
            titleContent = new GUIContent("Add Content");
            minSize = new Vector2(500, 400);
            maxSize = new Vector2(500, 400);
        }

        private IList<BuildingTemplate> m_templates;

        public delegate void AddEntityDelegate(AddTemplateResult<BuildingTemplate> data);

        public event AddEntityDelegate OnComplete;

        private void OnDestroy()
        {
            OnComplete?.Invoke(new AddTemplateResult<BuildingTemplate>());
        }

        public void CreateGUI()
        {
            if (m_templates == null)
                return;

            DrawTitle("Content List");
            DrawSearch();
            DrawCustomList();
        }

        public void InitPopup(IList<BuildingTemplate> list)
        {
            m_templates = list;
            DrawTitle("Content List");
            DrawSearch();
            DrawCustomList();
        }

        protected override void SearchFilter(string prompt)
        {
            m_filter = prompt;
            DrawCustomList();
        }

        private void DrawCustomList()
        {
            DrawElementsList(
                m_templates,
                t => t.Name,
                t =>
                {
                    OnComplete?.Invoke(new AddTemplateResult<BuildingTemplate>(t));
                    Close();
                }
            );
        }
    }
}