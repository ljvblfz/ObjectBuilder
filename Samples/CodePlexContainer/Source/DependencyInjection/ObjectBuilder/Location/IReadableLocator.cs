using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IReadableLocator : IEnumerable<KeyValuePair<object, object>>
    {
        int Count { get; }
        IReadableLocator ParentLocator { get; }
        bool ReadOnly { get; }

        bool Contains(object key);
        IReadableLocator FindBy(Predicate<KeyValuePair<object, object>> predicate);
        TItem Get<TItem>();
        TItem Get<TItem>(object key);
        object Get(object key);
    }
}