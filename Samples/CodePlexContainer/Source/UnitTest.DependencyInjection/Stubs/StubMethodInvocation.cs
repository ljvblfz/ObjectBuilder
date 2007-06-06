using System.Reflection;

namespace CodePlex.DependencyInjection
{
    public class StubMethodInvocation : IMethodInvocation
    {
        // Fields

        IParameterCollection inputs;
        IParameterCollection arguments;
        object target;
        MethodBase methodBase;

        public IParameterCollection Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }

        public IParameterCollection Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        public object Target
        {
            get { return target; }
            set { target = value; }
        }

        public MethodBase MethodBase
        {
            get { return methodBase; }
            set { methodBase = value; }
        }
    }
}