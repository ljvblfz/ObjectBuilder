using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class EventArgs<TData> : EventArgs
    {
        readonly TData data;

        public EventArgs(TData data)
        {
            Guard.ArgumentNotNull(data, "data");

            this.data = data;
        }

        public TData Data
        {
            get { return data; }
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }
}