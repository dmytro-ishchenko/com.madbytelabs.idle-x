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
        bool TryGetBuildingTemplate(BuildingType type, out BuildingTemplate buildingTemplate);
        bool TryGetBuildingContext(string id, out BuildingContext buildingContext);
        bool TryGetGameResource(string id, out GameResourcesTemplate gameResource);
    }
}