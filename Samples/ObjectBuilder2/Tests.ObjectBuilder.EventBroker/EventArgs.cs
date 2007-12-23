using System;

namespace ObjectBuilder
{
    class EventArgs<T> : EventArgs
    {
        readonly T data;

        public EventArgs(T data)
        {
            this.data = data;
        }

        public T Data
        {
            get { return data; }
        }
    }
}