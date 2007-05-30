using System;
using System.Collections;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class LifetimeContainer : ILifetimeContainer
    {
        // Fields

        List<object> items = new List<object>();

        // Lifetime

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                List<object> itemsCopy = new List<object>(items);
                itemsCopy.Reverse();

                foreach (object o in itemsCopy)
                {
                    IDisposable d = o as IDisposable;

                    if (d != null)
                        d.Dispose();
                }

                items.Clear();
            }
        }

        // Properties

        public int Count
        {
            get { return items.Count; }
        }

        // Methods

        public void Add(object item)
        {
            items.Add(item);
        }

        public bool Contains(object item)
        {
            return items.Contains(item);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(object item)
        {
            if (!items.Contains(item))
                return;

            items.Remove(item);
        }
    }
}