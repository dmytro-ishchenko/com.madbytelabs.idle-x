using Common.Interface;
using Common.Model;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
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
            if (!m_assetLibrary.TryGetContentTemplate(contentId, out ContentTemplate contentTemplate))
            {
                Debug.LogError($"Content {contentId} not found");
                return null;
            }

            var view = Object.Instantiate(contentTemplate.View);

            var component = view.GetComponent<ContentView>();

            component.Init(new ContentModel(contentTemplate, 0));


            return view.GetComponent<ContentView>();
        }
    }
}