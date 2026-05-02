using Data.ContentLibrary.Templates;
using Data.Persistent;
using UnityEngine;

namespace Data.Loader
{
    internal class UserDataLoader : IDataLoader<SaveModel>
    {
        public SaveModel Load()
        {
            var save = Resources.Load<SaveTemplate>("SaveTemplate");
            return save.SaveModel;
        }

        public void Save(SaveModel data)
        {
        }
    }
}