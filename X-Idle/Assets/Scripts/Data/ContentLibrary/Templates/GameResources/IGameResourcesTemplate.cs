using Common.Interface;
using Data.Interface;
using UnityEngine;

namespace Data.ContentLibrary.Templates.GameResources
{
    public interface IGameResourcesTemplate : ITemplate
    {
        Sprite Icon { get; }
    }
}