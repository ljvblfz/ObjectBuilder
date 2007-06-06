using System.Reflection;

namespace CodePlex.DependencyInjection
{
    public class MethodInvocation : IMethodInvocation
    {
        // Fields

        InputParameterCollection inputParams;
        ParameterCollection allParams;
        object target;
        object[] arguments;
        MethodBase methodBase;

        // Lifetime

        public MethodInvocation(object target,
                                MethodBase methodBase,
                                object[] arguments)
        {
            this.target = target;
            this.methodBase = methodBase;
            this.arguments = arguments;

            ParameterInfo[] paramInfos = methodBase.GetParameters();
            inputParams = new InputParameterCollection(arguments, paramInfos);
            allParams = new ParameterCollection(arguments, paramInfos);
        }

        // Properties

        IParameterCollection IMethodInvocation.Arguments
        {
            get { return allParams; }
        }

        public object[] Arguments
        {
            get { return arguments; }
        }

        public IParameterCollection Inputs
        {
            get { return inputParams; }
        }

        public MethodBase MethodBase
        {
            get { return methodBase; }
        }

        public object Target
        {
            get { return target; }
        }
    }
}