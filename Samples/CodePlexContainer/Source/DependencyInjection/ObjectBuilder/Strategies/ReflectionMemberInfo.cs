using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ReflectionMemberInfo<TMemberInfo> : IReflectionMemberInfo<TMemberInfo>
        where TMemberInfo : MethodBase
    {
        // Fields

        TMemberInfo memberInfo;

        // Lifetime

        public ReflectionMemberInfo(TMemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        // Properties

        public TMemberInfo MemberInfo
        {
            get { return memberInfo; }
        }

        public string Name
        {
            get { return memberInfo.Name; }
        }

        // Methods

        public object[] GetCustomAttributes(Type attributeType,
                                            bool inherit)
        {
            return memberInfo.GetCustomAttributes(attributeType, inherit);
        }

        public ParameterInfo[] GetParameters()
        {
            return memberInfo.GetParameters();
        }
    }
}