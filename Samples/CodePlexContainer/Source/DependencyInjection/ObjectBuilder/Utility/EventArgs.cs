using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class EventArgs<TData> : EventArgs
    {
        // Fields

        TData data;

        // Lifetime

        public EventArgs(TData data)
        {
            Guard.ArgumentNotNull(data, "data");

            this.data = data;
        }

        // Properties

        public TData Data
        {
            get { return data; }
        }

        // Methods

        public override string ToString()
        {
            return data.ToString();
        }
    }
}