using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.Interface;
using Data.Model;
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

        public IBuildingView GetView(string placeHolderId, string contentId)
        {
            if (!m_assetLibrary.TryGetBuildingTemplate(contentId, out BuildingTemplate contentTemplate))
            {
                Debug.LogError($"Content {contentId} not found");
                return null;
            }

            var view = Object.Instantiate(contentTemplate.View);

            var component = view.GetComponent<BuildingView>();

            component.Init(new BuildingModel(placeHolderId, contentTemplate, 0));


            return view.GetComponent<BuildingView>();
        }
    }
}