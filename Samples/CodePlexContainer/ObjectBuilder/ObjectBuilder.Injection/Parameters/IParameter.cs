using System;

namespace ObjectBuilder
{
    public interface IParameter
    {
        Type GetParameterType(IBuilderContext context);
        object GetValue(IBuilderContext context);
    }
}