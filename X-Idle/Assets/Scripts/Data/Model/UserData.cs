using System;
using System.Collections.Generic;
using Data.ContentLibrary;
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
                UserPlaceHolderData.AddPlaceHolder(placeholder.Id, new PlaceholderModel(placeholder.Id, placeholder.BuildingTemplateId, placeholder.PlaceHolderStatus, placeholder.PlaceHolderType));
            }
        }

        public UserBuildingsData UserBuildingsData { get; } = new();
        public UserResources UserResources { get; } = new();
        public UserPlaceHolderData UserPlaceHolderData { get; } = new();

        public SaveModel ToSave()
        {
            List<PlaceHolderSaveModel> placeholders = new();

            foreach (var buildingContext in UserPlaceHolderData.PlaceHolderDataMap.Values)
            {
                placeholders.Add(new PlaceHolderSaveModel(buildingContext.Id, "", buildingContext.PlaceHolderType, buildingContext.Status));
            }


            List<BuildingSaveModel> buildings = new();

            foreach (var building in UserBuildingsData.BuildingsMap.Values)
            {
                buildings.Add(new BuildingSaveModel(building.Id, building.Template.Id, building.Level));
            }

            List<ResourceSaveModel> resources = new();

            foreach (var resource in UserResources.Resources)
            {
                resources.Add(new ResourceSaveModel(resource.Key, resource.Value));
            }

            DateTime now = DateTime.UtcNow;

            return new SaveModel(((DateTimeOffset)now).ToUnixTimeSeconds(), placeholders, buildings, resources);
        }
    }
}