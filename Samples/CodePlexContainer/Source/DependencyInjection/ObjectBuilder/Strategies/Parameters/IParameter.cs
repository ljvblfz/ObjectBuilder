using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IParameter
    {
        // Methods

        Type GetParameterType(IBuilderContext context);
        object GetValue(IBuilderContext context);
    }
}