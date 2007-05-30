namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IReadWriteLocator : IReadableLocator
    {
        // Methods

        void Add(object key,
                 object value);

        bool Remove(object key);
    }
}