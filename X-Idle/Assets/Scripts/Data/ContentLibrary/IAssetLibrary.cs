using UnityEngine;

namespace Data.ContentLibrary
{
    public interface IAssetLibrary
    {
        GameObject GetContent(string id);
    }
}