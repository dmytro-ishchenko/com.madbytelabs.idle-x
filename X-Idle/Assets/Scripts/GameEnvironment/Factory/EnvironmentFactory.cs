using Data.ContentLibrary;
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

        public IBuildingView GetView(string placeHolderId, IBuildingTemplate contentTemplate)
        {
            var view = Object.Instantiate(contentTemplate.View);

            var component = view.GetComponent<BuildingView>();

            component.Init(new BuildingModel(placeHolderId, contentTemplate, 0));


            return view.GetComponent<BuildingView>();
        }
    }
}