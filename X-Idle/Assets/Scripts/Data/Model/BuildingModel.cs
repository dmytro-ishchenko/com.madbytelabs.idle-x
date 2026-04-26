using Data.Interface;

namespace Data.Model
{
    public class BuildingModel
    {
        public BuildingModel(string id, IBuildingTemplate template, int level)
        {
            m_id = id;
            m_template = template;
            m_level = level;
        }

        private string m_id;
        private IBuildingTemplate m_template;
        private int m_level;

        public string Id => m_id;
        public IBuildingTemplate Template => m_template;
        public int Level => m_level;

        public void SetTemplate(IBuildingTemplate template)
        {
            m_template = template;
        }

        public void SetLevel(int level)
        {
            m_level = level;
        }
    }
}