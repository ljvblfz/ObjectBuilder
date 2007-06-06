using System.Reflection;

namespace CodePlex.DependencyInjection
{
    public interface IMethodInvocation
    {
        IParameterCollection Inputs { get; }
        IParameterCollection Arguments { get; }
        object Target { get; }
        MethodBase MethodBase { get; }
    }
}