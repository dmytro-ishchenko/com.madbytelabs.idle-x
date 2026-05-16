using Data.Persistent;
using Newtonsoft.Json;

namespace Data.Loader
{
    internal class JsonSerializer: ISerializer<string>
    {
        public string Serialize(SaveModel model)
        {
          return  JsonConvert.SerializeObject(model);
        }

        public SaveModel Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<SaveModel>(data);
        }
    }
}