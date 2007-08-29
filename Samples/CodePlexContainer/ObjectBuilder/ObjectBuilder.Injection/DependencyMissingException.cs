using System;
using System.Runtime.Serialization;

namespace ObjectBuilder
{
    [Serializable]
    public class DependencyMissingException : Exception
    {
        public DependencyMissingException(object buildKey)
            : base("Could not locate dependency for build key " + buildKey) {}

        protected DependencyMissingException(SerializationInfo info,
                                             StreamingContext context)
            : base(info, context) {}
    }
}