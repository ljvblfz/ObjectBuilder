using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CodePlex.NUnitExtensions
{
    [Serializable]
    public class AssertException : Exception, ISerializable
    {
        // Fields

        string userMessage;

        // Lifetime

        public AssertException() {}

        protected AssertException(SerializationInfo info,
                                  StreamingContext context)
            : base(info, context)
        {
            userMessage = info.GetString("UserMessage");
        }

        public AssertException(string userMessage)
            : base(userMessage)
        {
            this.userMessage = userMessage;
        }

        // Properties

        public override string StackTrace
        {
            get { return FilterStackTrace(base.StackTrace); }
        }

        public string UserMessage
        {
            get { return userMessage; }
            protected set { userMessage = value; }
        }

        // Methods

        protected static string FilterStackTrace(string stackTrace)
        {
            string[] originalTrace = stackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            List<string> results = new List<string>();

            foreach (string line in originalTrace)
                if (!line.StartsWith("   at CodePlex.NUnitExtensions.Assert."))
                    results.Add(line);

            return string.Join("\r\n", results.ToArray());
        }

        public new virtual void GetObjectData(SerializationInfo info,
                                              StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("UserMessage", userMessage);
        }
    }
}