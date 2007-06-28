using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class StubMethodInvocation : IMethodInvocation
    {
        // Fields

        IParameterCollection arguments;
        IParameterCollection inputs;
        MethodBase methodBase;
        object target;

        public IParameterCollection Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        public IParameterCollection Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }

        public MethodBase MethodBase
        {
            get { return methodBase; }
            set { methodBase = value; }
        }

        public object Target
        {
            get { return target; }
            set { target = value; }
        }
    }
}