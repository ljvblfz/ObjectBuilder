using System;
using System.Collections.Generic;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class Locator : ReadWriteLocator
    {
        // Fields

        WeakRefDictionary<object, object> references = new WeakRefDictionary<object, object>();

        // Lifetime

        public Locator()
            : this(null) {}

        public Locator(IReadableLocator parentLocator)
        {
            SetParentLocator(parentLocator);
        }

        // Properties

        public override int Count
        {
            get { return references.Count; }
        }

        // Methods

        public override void Add(object key,
                                 object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            references.Add(key, value);
        }

        public override bool Contains(object key,
                                      SearchMode options)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!Enum.IsDefined(typeof(SearchMode), options))
                throw new ArgumentException(Resources.InvalidEnumerationValue, "options");

            if (references.ContainsKey(key))
                return true;

            if (options == SearchMode.Up && ParentLocator != null)
                return ParentLocator.Contains(key, options);

            return false;
        }

        public override object Get(object key,
                                   SearchMode options)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!Enum.IsDefined(typeof(SearchMode), options))
                throw new ArgumentException(Resources.InvalidEnumerationValue, "options");

            if (references.ContainsKey(key))
                return references[key];

            if (options == SearchMode.Up && ParentLocator != null)
                return ParentLocator.Get(key, options);

            return null;
        }

        public override bool Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return references.Remove(key);
        }

        public override IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return references.GetEnumerator();
        }
    }
}