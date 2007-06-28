namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class ReadWriteLocator : ReadableLocator, IReadWriteLocator
    {
        protected ReadWriteLocator() {}

        protected ReadWriteLocator(IReadableLocator parentLocator)
            : base(parentLocator) {}

        public override bool ReadOnly
        {
            get { return false; }
        }

        public abstract void Add(object key,
                                 object value);

        public abstract bool Remove(object key);
    }
}