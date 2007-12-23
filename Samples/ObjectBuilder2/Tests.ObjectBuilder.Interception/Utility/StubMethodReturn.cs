using System;

namespace ObjectBuilder
{
    public class StubMethodReturn : IMethodReturn
    {
        Exception exception;
        IParameterCollection outputs;
        object returnValue;

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        public IParameterCollection Outputs
        {
            get { return outputs; }
            set { outputs = value; }
        }

        public object ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }
}