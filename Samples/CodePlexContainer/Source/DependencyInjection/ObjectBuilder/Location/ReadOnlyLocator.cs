using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ReadOnlyLocator : ReadableLocator
    {
        // Fields

        IReadableLocator innerLocator;

        // Lifetime

        public ReadOnlyLocator(IReadableLocator innerLocator)
        {
            if (innerLocator == null)
                throw new ArgumentNullException("innerLocator");

            this.innerLocator = innerLocator;
        }

        // Properties

        public override int Count
        {
            get { return innerLocator.Count; }
        }

        public override IReadableLocator ParentLocator
        {
            get { return new ReadOnlyLocator(innerLocator.ParentLocator); }
        }

        public override bool ReadOnly
        {
            get { return true; }
        }

        // Methods

        public override bool Contains(object key,
                                      SearchMode options)
        {
            return innerLocator.Contains(key, options);
        }

        public override object Get(object key,
                                   SearchMode options)
        {
            return innerLocator.Get(key, options);
        }

        public override IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return innerLocator.GetEnumerator();
        }
    }
}