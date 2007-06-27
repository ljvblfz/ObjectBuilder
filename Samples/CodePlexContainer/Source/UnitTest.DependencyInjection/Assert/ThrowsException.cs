using System;
using System.Runtime.Serialization;

namespace CodePlex.NUnitExtensions
{
    [Serializable]
    public class ThrowsException : AssertActualExpectedException
    {
        // Fields

        string stackTrace = null;

        // Lifetime

        public ThrowsException(Type expectedType)
            : this("(No exception was thrown)", expectedType, null) {}

        public ThrowsException(Type expectedType,
                               string stackTrace)
            : this("(No exception was thrown)", expectedType, stackTrace) {}

        public ThrowsException(Exception actual,
                               Type expectedType)
            : this(actual == null ? null : actual.GetType().FullName, expectedType, null) {}

        public ThrowsException(Exception actual,
                               Type expectedType,
                               string stackTrace)
            : this(actual == null ? null : actual.GetType().FullName, expectedType, stackTrace) {}

        ThrowsException(string expected,
                        Type actual,
                        string stackTrace)
            : base(expected, actual.FullName, "Assert.Throws() Failure")
        {
            this.stackTrace = stackTrace;
        }

        protected ThrowsException(SerializationInfo info,
                                  StreamingContext context)
            : base(info, context) {}

        // Properties

        public override string StackTrace
        {
            get { return FilterStackTrace(stackTrace ?? base.StackTrace); }
        }
    }
}