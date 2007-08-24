using System;
using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    public class StubMethodReturn : IMethodReturn
    {
        // Fields

        Exception exception;
        IParameterCollection outputs;
        object returnValue;

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

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
    }
}