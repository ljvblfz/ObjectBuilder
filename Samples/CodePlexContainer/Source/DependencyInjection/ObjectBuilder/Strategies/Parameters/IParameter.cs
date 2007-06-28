using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IParameter
    {
        Type GetParameterType(IBuilderContext context);
        object GetValue(IBuilderContext context);
    }
}