using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ReflectionMemberInfo<TMemberInfo> : IReflectionMemberInfo<TMemberInfo>
        where TMemberInfo : MethodBase
    {
        readonly TMemberInfo memberInfo;

        public ReflectionMemberInfo(TMemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public TMemberInfo MemberInfo
        {
            get { return memberInfo; }
        }

        public string Name
        {
            get { return memberInfo.Name; }
        }

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