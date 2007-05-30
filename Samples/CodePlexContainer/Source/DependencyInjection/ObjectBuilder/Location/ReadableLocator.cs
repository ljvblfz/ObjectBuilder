using System;
using System.Collections;
using System.Collections.Generic;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class ReadableLocator : IReadableLocator
    {
        // Fields

        IReadableLocator parentLocator;

        // Properties

        public abstract int Count { get; }

        public virtual IReadableLocator ParentLocator
        {
            get { return parentLocator; }
        }

        public abstract bool ReadOnly { get; }

        // Methods

        public bool Contains(object key)
        {
            return Contains(key, SearchMode.Up);
        }

        public abstract bool Contains(object key,
                                      SearchMode options);

        public IReadableLocator FindBy(Predicate<KeyValuePair<object, object>> predicate)
        {
            return FindBy(SearchMode.Up, predicate);
        }

        public IReadableLocator FindBy(SearchMode options,
                                       Predicate<KeyValuePair<object, object>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (!Enum.IsDefined(typeof(SearchMode), options))
                throw new ArgumentException(Resources.InvalidEnumerationValue, "options");

            Locator results = new Locator();
            IReadableLocator currentLocator = this;

            while (currentLocator != null)
            {
                FindInLocator(predicate, results, currentLocator);
                currentLocator = options == SearchMode.Local ? null : currentLocator.ParentLocator;
            }

            return new ReadOnlyLocator(results);
        }

        void FindInLocator(Predicate<KeyValuePair<object, object>> predicate,
                           Locator results,
                           IReadableLocator currentLocator)
        {
            foreach (KeyValuePair<object, object> kvp in currentLocator)
            {
                if (!results.Contains(kvp.Key) && predicate(kvp))
                {
                    results.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public TItem Get<TItem>()
        {
            return (TItem)Get(typeof(TItem));
        }

        public TItem Get<TItem>(object key)
        {
            return (TItem)Get(key);
        }

        public TItem Get<TItem>(object key,
                                SearchMode options)
        {
            return (TItem)Get(key, options);
        }

        public object Get(object key)
        {
            return Get(key, SearchMode.Up);
        }

        public abstract object Get(object key,
                                   SearchMode options);

        public abstract IEnumerator<KeyValuePair<object, object>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void SetParentLocator(IReadableLocator parentLocator)
        {
            this.parentLocator = parentLocator;
        }
    }
}