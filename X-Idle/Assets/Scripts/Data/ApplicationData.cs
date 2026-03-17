using Common;
using Data.ContentLibrary;
using UnityEditor;

namespace Data
{
    internal class ApplicationData : IApplicationData
    {
        private IAssetLibrary m_assetLibrary;

        public IAssetLibrary AssetLibrary
        {
            get
            {
                if (m_assetLibrary == null)
                    m_assetLibrary = (AssetLibrary)AssetDatabase.LoadAssetAtPath(AssetPath.LIBRARY_PATH, typeof(AssetLibrary));
                return m_assetLibrary;
            }
        }
    }
}