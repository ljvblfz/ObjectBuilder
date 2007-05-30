using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class LifetimeEventArgs : EventArgs
    {
        // Fields

        object item;

        // Lifetime

        public LifetimeEventArgs(object item)
        {
            this.item = item;
        }

        // Properties

        public object Item
        {
            get { return item; }
        }
    }
}