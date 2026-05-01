using Data.Persistent;
using UnityEngine;

namespace Data.ContentLibrary.Templates
{
    public class SaveTemplate : ScriptableObject
    {
        [SerializeField] private SaveModel m_saveModel;
        public SaveModel SaveModel => m_saveModel;

        public void InitSaveModel(SaveModel saveModel)
        {
            m_saveModel = saveModel;
        }
    }
}