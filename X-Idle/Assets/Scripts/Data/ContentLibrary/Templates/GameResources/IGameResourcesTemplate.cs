using Data.Enum;
using Data.Interface;
using UnityEngine;

namespace Data.ContentLibrary.Templates.GameResources
{
    public interface IGameResourcesTemplate : ITemplate
    {
        ResourcesType ResourcesType { get; }
        Sprite Icon { get; }
    }
}