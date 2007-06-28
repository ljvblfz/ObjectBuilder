using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodPolicy : IBuilderPolicy
    {
        Dictionary<string, IMethodCallInfo> Methods { get; }
    }
}