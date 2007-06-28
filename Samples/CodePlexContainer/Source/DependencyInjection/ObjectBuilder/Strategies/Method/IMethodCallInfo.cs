using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodCallInfo
    {
        object[] GetParameters(IBuilderContext context,
                               Type type,
                               string id,
                               MethodInfo method);

        MethodInfo SelectMethod(IBuilderContext context,
                                Type type,
                                string id);
    }
}