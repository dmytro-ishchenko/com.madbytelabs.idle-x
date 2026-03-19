using Data.ContentLibrary.Templates.Context.Building;
using UnityEngine;

namespace Data.Interface
{
    public interface IBuildingTemplate : ITemplate
    {
        GameObject View { get; }
        BuildingContext BuildingContext { get; }
    }
}