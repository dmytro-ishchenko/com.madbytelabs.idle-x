namespace Data.Loader
{
    internal interface IDataLoader<T>
    {
        T Load();
        void Save(T data);
    }
}