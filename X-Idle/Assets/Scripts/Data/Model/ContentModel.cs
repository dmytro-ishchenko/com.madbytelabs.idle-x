using Common.Interface;
using Data.Interface;

namespace Common.Model
{
    public class ContentModel
    {
        public ContentModel(IContentTemplate template, int level)
        {
            m_id = template.Id;
            m_template = template;
            m_level = level;
        }

        private string m_id;
        private string m_placeHolderId;
        private IContentTemplate m_template;
        private int m_level;

        public string Id => m_id;
        public IContentTemplate Template => m_template;
        public int Level => m_level;
    }
}