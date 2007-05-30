using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IReflectionMemberInfo<TMemberInfo>
    {
        // Properties

        TMemberInfo MemberInfo { get; }
        string Name { get; }

        // Methods

        object[] GetCustomAttributes(Type attributeType,
                                     bool inherit);

        ParameterInfo[] GetParameters();
    }
}