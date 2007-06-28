using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class Locator : ReadWriteLocator
    {
        readonly WeakRefDictionary<object, object> references = new WeakRefDictionary<object, object>();

        public Locator()
            : this(null) {}

        public Locator(IReadableLocator parentLocator)
            : base(parentLocator) {}

        public override int Count
        {
            get { return references.Count; }
        }

        public override void Add(object key,
                                 object value)
        {
            Guard.ArgumentNotNull(key, "key");
            Guard.ArgumentNotNull(value, "value");

            references.Add(key, value);
        }

        public override bool Contains(object key)
        {
            Guard.ArgumentNotNull(key, "key");

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

        public override IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return references.GetEnumerator();
        }

        public override bool Remove(object key)
        {
            Guard.ArgumentNotNull(key, "key");

            return references.Remove(key);
        }
    }
}