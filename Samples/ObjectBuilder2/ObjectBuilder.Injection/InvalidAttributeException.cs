using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ObjectBuilder
{
    [Serializable]
    public class InvalidAttributeException : Exception
    {
        public InvalidAttributeException(Type type,
                                         string memberName)
            : base(string.Format(CultureInfo.CurrentCulture,
                                 "Too many dependency injection attributes defined on {0}.{1}.",
                                 type,
                                 memberName)) {}

        protected InvalidAttributeException(SerializationInfo info,
                                            StreamingContext context)
            : base(info, context) {}
    }
}