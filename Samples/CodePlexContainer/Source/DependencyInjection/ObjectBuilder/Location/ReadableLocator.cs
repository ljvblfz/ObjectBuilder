using System;
using System.Collections;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class ReadableLocator : IReadableLocator
    {
        readonly IReadableLocator parentLocator;

        protected ReadableLocator() {}

        protected ReadableLocator(IReadableLocator parentLocator)
        {
            this.parentLocator = parentLocator;
        }

        public abstract int Count { get; }

        public virtual IReadableLocator ParentLocator
        {
            get { return parentLocator; }
        }

        public abstract bool ReadOnly { get; }

        public abstract bool Contains(object key);

        public IReadableLocator FindBy(Predicate<KeyValuePair<object, object>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            Locator results = new Locator();
            IReadableLocator currentLocator = this;

            while (currentLocator != null)
            {
                FindInLocator(predicate, results, currentLocator);
                currentLocator = currentLocator.ParentLocator;
            }

            return new ReadOnlyLocator(results);
        }

        static void FindInLocator(Predicate<KeyValuePair<object, object>> predicate,
                                  IReadWriteLocator results,
                                  IEnumerable<KeyValuePair<object, object>> currentLocator)
        {
            foreach (KeyValuePair<object, object> kvp in currentLocator)
                if (!results.Contains(kvp.Key) && predicate(kvp))
                    results.Add(kvp.Key, kvp.Value);
        }

        public TItem Get<TItem>()
        {
            return (TItem)Get(typeof(TItem));
        }

        public TItem Get<TItem>(object key)
        {
            return (TItem)Get(key);
        }

        public abstract object Get(object key);

        public abstract IEnumerator<KeyValuePair<object, object>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}