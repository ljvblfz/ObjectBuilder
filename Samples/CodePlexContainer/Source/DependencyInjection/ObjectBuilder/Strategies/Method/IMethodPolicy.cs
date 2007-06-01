using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodPolicy : IBuilderPolicy
    {
        // Properties

        Dictionary<string, IMethodCallInfo> Methods { get; }
    }
}