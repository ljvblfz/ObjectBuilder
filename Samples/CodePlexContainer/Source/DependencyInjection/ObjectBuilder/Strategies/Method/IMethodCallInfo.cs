using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodCallInfo
    {
        // Methods

        object[] GetParameters(IBuilderContext context,
                               Type type,
                               string id,
                               MethodInfo method);

        MethodInfo SelectMethod(IBuilderContext context,
                                Type type,
                                string id);
    }
}