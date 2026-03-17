using Data.ContentLibrary;

namespace Data
{
    public interface IApplicationData
    {
        IAssetLibrary AssetLibrary { get; }
    }
}