using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class MethodReflectionStrategy : ReflectionStrategy<MethodInfo>
    {
        protected override void AddParametersToPolicy(IBuilderContext context,
                                                      object buildKey,
                                                      IMemberInfo<MethodInfo> member,
                                                      IEnumerable<IParameter> parameters)
        {
            IMethodCallPolicy result = context.Policies.Get<IMethodCallPolicy>(buildKey);

            if (result == null)
            {
                result = new MethodCallPolicy();
                context.Policies.Set(result, buildKey);
            }

            result.Methods.Add(new ReflectionMethodCallInfo(member.MemberInfo, parameters));
        }

        protected override IEnumerable<IMemberInfo<MethodInfo>> GetMembers(IBuilderContext context,
                                                                           object buildKey,
                                                                           object existing)
        {
            foreach (MethodInfo method in GetTypeFromBuildKey(buildKey).GetMethods())
                yield return new MethodMemberInfo<MethodInfo>(method);
        }

        protected override bool MemberRequiresProcessing(IMemberInfo<MethodInfo> member)
        {
            return (member.GetCustomAttributes(typeof(InjectionMethodAttribute), true).Length > 0);
        }
    }
}