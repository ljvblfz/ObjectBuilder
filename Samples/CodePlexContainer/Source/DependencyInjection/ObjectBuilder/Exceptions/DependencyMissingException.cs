using System;
using System.Runtime.Serialization;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [Serializable]
    public class DependencyMissingException : Exception
    {
        // Lifetime

        public DependencyMissingException() {}

        public DependencyMissingException(string message)
            : base(message) {}

        public DependencyMissingException(string message,
                                          Exception exception)
            : base(message, exception) {}

        protected DependencyMissingException(SerializationInfo info,
                                             StreamingContext context)
            : base(info, context) {}
    }
}