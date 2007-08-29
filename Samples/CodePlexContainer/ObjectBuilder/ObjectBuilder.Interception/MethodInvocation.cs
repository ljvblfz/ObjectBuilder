using System.Reflection;

namespace ObjectBuilder
{
    public class MethodInvocation : IMethodInvocation
    {
        readonly ParameterCollection allParams;
        readonly object[] arguments;
        readonly InputParameterCollection inputParams;
        readonly MethodBase methodBase;
        readonly object target;

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