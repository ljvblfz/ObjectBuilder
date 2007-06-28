using System;
using System.Globalization;
using System.Runtime.Serialization;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [Serializable]
    public class InvalidAttributeException : Exception
    {
        public InvalidAttributeException() {}

        public InvalidAttributeException(string message)
            : base(message) {}

        public InvalidAttributeException(string message,
                                         Exception exception)
            : base(message, exception) {}

        public InvalidAttributeException(Type type,
                                         string memberName)
            : base(String.Format(CultureInfo.CurrentCulture, Resources.InvalidAttributeCombination, type, memberName)) {}

        protected InvalidAttributeException(SerializationInfo info,
                                            StreamingContext context)
            : base(info, context) {}
    }
}