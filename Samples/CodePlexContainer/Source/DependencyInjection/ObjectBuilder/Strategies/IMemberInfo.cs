using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMemberInfo<TMemberInfo>
    {
        TMemberInfo MemberInfo { get; }
        string Name { get; }

        object[] GetCustomAttributes(Type attributeType,
                                     bool inherit);

        ParameterInfo[] GetParameters();
    }
}