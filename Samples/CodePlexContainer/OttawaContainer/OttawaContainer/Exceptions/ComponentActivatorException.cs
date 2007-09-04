using System;

namespace Ottawa
{
    public class ComponentActivatorException : Exception
    {
        public ComponentActivatorException(Type type)
            : base("Cannot create an instance of type " + type.FullName) {}
    }
}