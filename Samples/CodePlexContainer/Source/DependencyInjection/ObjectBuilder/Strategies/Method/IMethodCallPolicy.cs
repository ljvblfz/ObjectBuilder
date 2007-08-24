using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodCallPolicy : IBuilderPolicy
    {
        List<IMethodCallInfo> Methods { get; }
    }
}