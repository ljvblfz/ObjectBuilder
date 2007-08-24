using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PropertyReflectionStrategy : ReflectionStrategy<PropertyInfo>
    {
        protected override void AddParametersToPolicy(IBuilderContext context,
                                                      object buildKey,
                                                      IMemberInfo<PropertyInfo> member,
                                                      IEnumerable<IParameter> parameters)
        {
            IPropertySetterPolicy result = context.Policies.Get<IPropertySetterPolicy>(buildKey);

            if (result == null)
            {
                result = new PropertySetterPolicy();
                context.Policies.Set(result, buildKey);
            }

            foreach (IParameter parameter in parameters)
                result.Properties.Add(new ReflectionPropertySetterInfo(member.MemberInfo, parameter));
        }

        protected override IEnumerable<IMemberInfo<PropertyInfo>> GetMembers(IBuilderContext context,
                                                                             object buildKey,
                                                                             object existing)
        {
            foreach (PropertyInfo propInfo in GetTypeFromBuildKey(buildKey).GetProperties())
                yield return new PropertyMemberInfo(propInfo);
        }

        protected override bool MemberRequiresProcessing(IMemberInfo<PropertyInfo> member)
        {
            return (member.GetCustomAttributes(typeof(ParameterAttribute), true).Length > 0);
        }
    }
}