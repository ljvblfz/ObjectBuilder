using System;
using System.Runtime.Serialization;

namespace CodePlex.NUnitExtensions
{
    [Serializable]
    public class ThrowsException : AssertActualExpectedException
    {
        readonly string stackTrace = null;

        public ThrowsException(Type expectedType)
            : this("(No exception was thrown)", expectedType, null, null) {}

        public ThrowsException(Type expectedType,
                               string stackTrace)
            : this("(No exception was thrown)", expectedType, null, stackTrace) {}

        public ThrowsException(Exception actual,
                               Type expectedType)
            : this(actual == null ? null : actual.GetType().FullName, expectedType, actual == null ? null : actual.Message, null) {}

        public ThrowsException(Exception actual,
                               Type expectedType,
                               string stackTrace)
            : this(actual == null ? null : actual.GetType().FullName, expectedType, actual == null ? null : actual.Message, stackTrace) {}

        ThrowsException(string actual,
                        Type expected,
                        string actualMessage,
                        string stackTrace)
            : base(actual + (actualMessage == null ? "" : ": " + actualMessage), expected.FullName, "Assert.Throws() Failure")
        {
            this.stackTrace = stackTrace;
        }

        protected ThrowsException(SerializationInfo info,
                                  StreamingContext context)
            : base(info, context) {}

        public override string StackTrace
        {
            get { return FilterStackTrace(stackTrace ?? base.StackTrace); }
        }
    }
}