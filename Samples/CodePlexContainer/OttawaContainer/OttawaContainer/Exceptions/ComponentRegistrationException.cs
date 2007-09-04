using System;
using System.Runtime.Serialization;

namespace Ottawa
{
    public class ComponentRegistrationException : Exception
    {
        public ComponentRegistrationException(string key)
            : base("The key '" + key + "' already has a registered component") {}

        public ComponentRegistrationException(SerializationInfo info,
                                              StreamingContext context)
            : base(info, context) {}
    }
}