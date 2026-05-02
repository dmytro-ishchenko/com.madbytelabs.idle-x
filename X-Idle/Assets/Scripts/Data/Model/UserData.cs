using System.Collections.Generic;
using Data.ContentLibrary;
using Data.ContentLibrary.Templates;
using Data.Persistent;

namespace Data.Model
{
    internal class UserData
    {
        public UserData(UserPlaceHolderData placeHolderData, List<BuildingModel> buildings)
        {
            UserPlaceHolderData = placeHolderData;

            if (buildings is { Count: > 0 })
            {
                foreach (var building in buildings)
                {
                    UserBuildingsData.AddStartBuildingModel(building.Id, building);
                }
            }
        }

        public UserData(IAssetLibrary assetLibrary, SaveModel saveModel)
        {
            foreach (var resource in saveModel.Resources)
            {
                UserResources.SetGameResource(resource.ResourceType, resource.Amount);
            }

            foreach (var building in saveModel.Buildings)
            {
                assetLibrary.TryGetBuildingTemplate(building.BuildingTemplateId, out var buildingTemplate);
                UserBuildingsData.AddStartBuildingModel(building.Id, new BuildingModel(building.Id, buildingTemplate, building.Level));
            }

            foreach (var placeholder in saveModel.PlaceHolders)
            {
                UserPlaceHolderData.AddPlaceHolder(placeholder.Id, new PlaceholderModel(placeholder.Id, placeholder.PlaceHolderStatus, placeholder.PlaceHolderType));
            }
        }

        public UserBuildingsData UserBuildingsData { get; } = new();
        public UserResources UserResources { get; } = new();
        public UserPlaceHolderData UserPlaceHolderData { get; } = new();

        public SaveModel ToSave(SceneTemplate sceneTemplate)
        {
            List<PlaceHolderSaveModel> placeholders = new();

            // foreach (var buildingContext in sceneTemplate.SceneBuildings)
            // {
            //
            //
            //     placeHolderData.AddPlaceHolder(buildingContext.Id, new PlaceholderModel(buildingContext.Id, buildingContext.PlaceHolderStatus, buildingContext.PlaceHolderType));
            // }
            //
            // foreach (var element in UserPlaceHolderData.PlaceHolderDataMap)
            // {
            //     placeholders.Add(new PlaceHolderSaveModel(element.Value.Id,element.Value.,element.Value.Status, element.Value.PlaceHolderType));
            // }
            
            List<BuildingSaveModel> buildings = new();
            
            List<ResourceSaveModel> resources = new();

            return new SaveModel(placeholders, buildings, resources);
        }
    }
}