using System;
using System.Runtime.Serialization;

namespace Ottawa
{
    [Serializable]
    public class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(string key)
            : base("Could not resolve dependency for key '" + key + "'") {}

        public ComponentNotFoundException(Type service)
            : base("Could not resolve dependency for service " + service.FullName) {}

        protected ComponentNotFoundException(SerializationInfo info,
                                             StreamingContext context) {}
    }
}