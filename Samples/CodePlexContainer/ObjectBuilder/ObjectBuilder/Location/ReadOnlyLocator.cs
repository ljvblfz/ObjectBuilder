using System.Collections.Generic;

namespace ObjectBuilder
{
    public class ReadOnlyLocator : ReadableLocator
    {
        readonly IReadableLocator innerLocator;

        public ReadOnlyLocator(IReadableLocator innerLocator)
        {
            Guard.ArgumentNotNull(innerLocator, "innerLocator");

            this.innerLocator = innerLocator;
        }

        public override int Count
        {
            get { return innerLocator.Count; }
        }

        public override IReadableLocator ParentLocator
        {
            get { return innerLocator.ParentLocator == null ? null : new ReadOnlyLocator(innerLocator.ParentLocator); }
        }

        public override bool ReadOnly
        {
            get { return true; }
        }

        public override bool Contains(object key)
        {
            return innerLocator.Contains(key);
        }

        public override object Get(object key)
        {
            return innerLocator.Get(key);
        }

        public override IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return innerLocator.GetEnumerator();
        }
    }
}