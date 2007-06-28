using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class LifetimeEventArgs : EventArgs
    {
        readonly object item;

        public LifetimeEventArgs(object item)
        {
            this.item = item;
        }

        public object Item
        {
            get { return item; }
        }
    }
}