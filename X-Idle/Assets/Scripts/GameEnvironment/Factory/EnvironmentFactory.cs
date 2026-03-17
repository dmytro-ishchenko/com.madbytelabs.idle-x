using Common.Interface;
using Data.ContentLibrary;
using GameEnvironment.View;
using UnityEngine;

namespace GameEnvironment.Factory
{
    internal class EnvironmentFactory : IEnvironmentFactory
    {
        internal EnvironmentFactory(IAssetLibrary assetLibrary)
        {
            m_assetLibrary = assetLibrary;
        }

        private readonly IAssetLibrary m_assetLibrary;

        public IContentView GetView(string contentId)
        {
            var content = m_assetLibrary.GetContent(contentId);

            if (content == null)
            {
                Debug.LogError($"Content {contentId} not found");
                return null;
            }

            return content.AddComponent<ContentView>();
        }
    }
}