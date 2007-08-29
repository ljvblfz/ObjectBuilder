using System.Collections.Generic;

namespace ObjectBuilder
{
    public interface IMethodCallPolicy : IBuilderPolicy
    {
        List<IMethodCallInfo> Methods { get; }
    }
}