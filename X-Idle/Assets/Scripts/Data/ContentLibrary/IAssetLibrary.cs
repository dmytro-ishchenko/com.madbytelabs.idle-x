using System.Collections.Generic;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;

namespace Data.ContentLibrary
{
    public interface IAssetLibrary
    {
        Dictionary<string, ContentTemplate> ContentMap { get; }
        Dictionary<string, BuildingContext> ContextMap { get; }
        Dictionary<string, GameResourcesTemplate> GameResourcesMap { get; }
        bool TryGetContentTemplate(string id, out ContentTemplate contentTemplate);
        bool TryGetBuildingContext(string id, out BuildingContext buildingContext);
        bool TryGetGameResource(string id, out GameResourcesTemplate gameResource);
    }
}