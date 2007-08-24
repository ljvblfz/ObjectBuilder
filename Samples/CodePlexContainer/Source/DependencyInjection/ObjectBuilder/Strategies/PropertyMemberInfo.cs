using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PropertyMemberInfo : IMemberInfo<PropertyInfo>
    {
        readonly PropertyInfo prop;

        public PropertyMemberInfo(PropertyInfo prop)
        {
            this.prop = prop;
        }

        public PropertyInfo MemberInfo
        {
            get { return prop; }
        }

        public string Name
        {
            get { return prop.Name; }
        }

        public object[] GetCustomAttributes(Type attributeType,
                                            bool inherit)
        {
            return prop.GetCustomAttributes(attributeType, inherit);
        }

        public ParameterInfo[] GetParameters()
        {
            return new ParameterInfo[] { new CustomPropertyParameterInfo(prop) };
        }

        class CustomPropertyParameterInfo : ParameterInfo
        {
            readonly PropertyInfo prop;

            public CustomPropertyParameterInfo(PropertyInfo prop)
            {
                this.prop = prop;
            }

            public override Type ParameterType
            {
                get { return prop.PropertyType; }
            }

            public override object[] GetCustomAttributes(Type attributeType,
                                                         bool inherit)
            {
                return prop.GetCustomAttributes(attributeType, inherit);
            }
        }
    }
}