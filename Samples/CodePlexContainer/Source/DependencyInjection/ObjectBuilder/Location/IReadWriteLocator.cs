namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IReadWriteLocator : IReadableLocator
    {
        void Add(object key,
                 object value);

        bool Remove(object key);
    }
}