using System.IO;
using Data.Persistent;

namespace Data.Loader
{
    internal class UserDataLoader : IDataLoader<SaveModel>
    {
        private ISerializer<string> m_serializer = new JsonSerializer();

        readonly string m_dataPath = Path.Combine(DataPath.UserDataPath, "SaveModel.json");

        public SaveModel Load()
        {
            SaveModel model = null;

            if (File.Exists(m_dataPath))
            {
                string jsonString = File.ReadAllText(m_dataPath);
                model = m_serializer.Deserialize(jsonString);
            }
            else
            {
                // var save = Resources.Load<SaveTemplate>("SaveTemplate");
                //  model = save.SaveModel;
            }

            return model;
        }

        public void Save(SaveModel data)
        {
            var json = m_serializer.Serialize(data);
            if (!Directory.Exists(DataPath.UserDataPath))
                Directory.CreateDirectory(DataPath.UserDataPath);

            File.WriteAllText(m_dataPath, json);
        }
    }
}