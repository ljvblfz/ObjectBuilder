using System;
using System.Runtime.Serialization;

namespace CodePlex.NUnitExtensions
{
    public class AssertActualExpectedException : AssertException
    {
        readonly string actual;
        readonly string expected;

        protected AssertActualExpectedException(SerializationInfo info,
                                                StreamingContext context)
            : base(info, context)
        {
            actual = info.GetString("Actual");
            expected = info.GetString("Expected");
        }

        public AssertActualExpectedException(object actual,
                                             object expected,
                                             string userMessage)
            : base(userMessage)
        {
            this.actual = actual == null ? null : actual.ToString();
            this.expected = expected == null ? null : expected.ToString();
        }

        public string Actual
        {
            get { return actual; }
        }

        public string Expected
        {
            get { return expected; }
        }

        public override string Message
        {
            get
            {
                return string.Format("{0}\r\nExpected: {1}\r\nActual:   {2}",
                                     base.Message,
                                     FormatMultiLine(Expected ?? "(null)"),
                                     FormatMultiLine(Actual ?? "(null)"));
            }
        }

        static string FormatMultiLine(string value)
        {
            return value.Replace(Environment.NewLine, Environment.NewLine + "          ");
        }

        public override void GetObjectData(SerializationInfo info,
                                           StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Expected", expected);
            info.AddValue("Actual", actual);
        }
    }
}