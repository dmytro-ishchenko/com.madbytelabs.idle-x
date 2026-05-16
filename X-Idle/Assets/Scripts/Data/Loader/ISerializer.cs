using Data.Persistent;

namespace Data.Loader
{
    internal interface ISerializer<T>
    {
        T Serialize(SaveModel model);
        SaveModel Deserialize(T data);
    }
}