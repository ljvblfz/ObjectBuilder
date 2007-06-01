using System;
using System.Collections.Generic;

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
            : base(parentLocator) {}

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

        public override bool Contains(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (references.ContainsKey(key))
                return true;

            if (ParentLocator != null)
                return ParentLocator.Contains(key);

            return false;
        }

        public override object Get(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (references.ContainsKey(key))
                return references[key];

            if (ParentLocator != null)
                return ParentLocator.Get(key);

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