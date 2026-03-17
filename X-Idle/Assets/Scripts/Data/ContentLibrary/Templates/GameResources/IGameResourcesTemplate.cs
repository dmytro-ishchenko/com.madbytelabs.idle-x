using Common.Interface;
using UnityEngine;

namespace Data.ContentLibrary.Templates.GameResources
{
    public interface IGameResourcesTemplate : ITemplate
    {
        Sprite Icon { get; }
    }
}