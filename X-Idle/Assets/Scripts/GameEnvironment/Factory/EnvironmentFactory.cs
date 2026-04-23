using Data.Interface;
using Data.Model;
using GameEnvironment.View;
using UnityEngine;

namespace GameEnvironment.Factory
{
    internal class EnvironmentFactory : IEnvironmentFactory
    {
        public IBuildingView GetView(string placeHolderId, IBuildingTemplate contentTemplate)
        {
            var view = Object.Instantiate(contentTemplate.View);

            var component = view.GetComponent<BuildingView>();

            component.Init(new BuildingModel(placeHolderId, contentTemplate, 1));


            return view.GetComponent<BuildingView>();
        }
    }
}