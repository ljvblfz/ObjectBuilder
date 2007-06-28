using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PropertyReflectionStrategy : ReflectionStrategy<PropertyInfo>
    {
        protected override void AddParametersToPolicy(IBuilderContext context,
                                                      Type typeToBuild,
                                                      string idToBuild,
                                                      IReflectionMemberInfo<PropertyInfo> member,
                                                      IEnumerable<IParameter> parameters)
        {
            PropertySetterPolicy result = context.Policies.Get<IPropertySetterPolicy>(typeToBuild, idToBuild) as PropertySetterPolicy;

            if (result == null)
            {
                result = new PropertySetterPolicy();
                context.Policies.Set<IPropertySetterPolicy>(result, typeToBuild, idToBuild);
            }

            foreach (IParameter parameter in parameters)
                if (!result.Properties.ContainsKey(member.Name))
                    result.Properties.Add(member.Name, new PropertySetterInfo(member.MemberInfo, parameter));
        }

        protected override IEnumerable<IReflectionMemberInfo<PropertyInfo>> GetMembers(IBuilderContext context,
                                                                                       Type typeToBuild,
                                                                                       object existing,
                                                                                       string idToBuild)
        {
            foreach (PropertyInfo propInfo in typeToBuild.GetProperties())
                yield return new PropertyReflectionMemberInfo(propInfo);
        }

        protected override bool MemberRequiresProcessing(IReflectionMemberInfo<PropertyInfo> member)
        {
            return (member.GetCustomAttributes(typeof(ParameterAttribute), true).Length > 0);
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

        class PropertyReflectionMemberInfo : IReflectionMemberInfo<PropertyInfo>
        {
            readonly PropertyInfo prop;

            public PropertyReflectionMemberInfo(PropertyInfo prop)
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
        }
    }
}