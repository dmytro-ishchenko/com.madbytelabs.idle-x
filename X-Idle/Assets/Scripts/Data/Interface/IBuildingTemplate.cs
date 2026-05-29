using Data.ContentLibrary.Templates.Context.Building;
using UnityEngine;

namespace Data.Interface
{
    public interface IBuildingTemplate : ITemplate
    {
        GameObject View { get; }
        Sprite Icon { get; }
        Sprite IconWide { get; }
        BuildingContext BuildingContext { get; }
        string ShortDescription { get; }
    }
}