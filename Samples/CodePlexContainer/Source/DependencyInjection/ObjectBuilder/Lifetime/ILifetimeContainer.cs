using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface ILifetimeContainer : IEnumerable<object>, IDisposable
    {
        int Count { get; }

        void Add(object item);
        bool Contains(object item);
        void Remove(object item);
    }
}