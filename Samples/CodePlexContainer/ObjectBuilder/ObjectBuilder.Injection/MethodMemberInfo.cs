using System;
using System.Reflection;

namespace ObjectBuilder
{
    public class MethodMemberInfo<TMemberInfo> : IMemberInfo<TMemberInfo>
        where TMemberInfo : MethodBase
    {
        readonly TMemberInfo memberInfo;

        public MethodMemberInfo(TMemberInfo memberInfo)
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