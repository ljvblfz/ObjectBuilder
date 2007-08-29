using System.Reflection;

namespace ObjectBuilder
{
    public interface IMethodInvocation
    {
        IParameterCollection Arguments { get; }
        IParameterCollection Inputs { get; }
        MethodBase MethodBase { get; }
        object Target { get; }
    }
}