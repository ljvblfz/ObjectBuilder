namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class ReadWriteLocator : ReadableLocator, IReadWriteLocator
    {
        // Properties

        public override bool ReadOnly
        {
            get { return false; }
        }

        // Methods

        public abstract void Add(object key,
                                 object value);

        public abstract bool Remove(object key);
    }
}