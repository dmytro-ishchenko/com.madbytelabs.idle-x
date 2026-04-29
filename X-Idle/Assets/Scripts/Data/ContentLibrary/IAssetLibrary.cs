using System.Collections.Generic;
using Data.ContentLibrary.Templates;
using Data.ContentLibrary.Templates.Context.Building;
using Data.ContentLibrary.Templates.GameResources;
using Data.Enum;

namespace Data.ContentLibrary
{
    public interface IAssetLibrary
    {
        IList<BuildingTemplate> Buildings { get; }
        bool TryGetBuildingTemplate(string id, out BuildingTemplate buildingTemplate);
        bool TryGetGameResource(GameResourceType type, out GameResourcesTemplate gameResource);
    }
}