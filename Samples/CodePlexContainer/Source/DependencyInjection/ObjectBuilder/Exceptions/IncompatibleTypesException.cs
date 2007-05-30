using System;
using System.Runtime.Serialization;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [Serializable]
    public class IncompatibleTypesException : Exception
    {
        // Lifetime

        public IncompatibleTypesException() {}

        public IncompatibleTypesException(string message)
            : base(message) {}

        public IncompatibleTypesException(string message,
                                          Exception exception)
            : base(message, exception) {}

        protected IncompatibleTypesException(SerializationInfo info,
                                             StreamingContext context)
            : base(info, context) {}
    }
}