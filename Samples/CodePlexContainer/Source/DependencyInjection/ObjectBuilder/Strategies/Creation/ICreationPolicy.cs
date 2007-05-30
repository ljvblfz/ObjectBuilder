using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface ICreationPolicy : IBuilderPolicy
    {
        // Methods

        object[] GetParameters(IBuilderContext context,
                               Type type,
                               string id,
                               ConstructorInfo constructor);

        ConstructorInfo SelectConstructor(IBuilderContext context,
                                          Type type,
                                          string id);
    }
}