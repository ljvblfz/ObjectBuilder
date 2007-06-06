using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class StubMethodReturn : IMethodReturn
    {
        // Fields

        IParameterCollection outputs;
        object returnValue;
        Exception exception;

        // Properties

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

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }
    }
}