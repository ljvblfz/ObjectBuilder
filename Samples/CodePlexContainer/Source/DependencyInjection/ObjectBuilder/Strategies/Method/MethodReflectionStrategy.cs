using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class MethodReflectionStrategy : ReflectionStrategy<MethodInfo>
    {
        // Methods

        protected override void AddParametersToPolicy(IBuilderContext context,
                                                      Type typeToBuild,
                                                      string idToBuild,
                                                      IReflectionMemberInfo<MethodInfo> member,
                                                      IEnumerable<IParameter> parameters)
        {
            MethodPolicy result = context.Policies.Get<IMethodPolicy>(typeToBuild, idToBuild) as MethodPolicy;

            if (result == null)
            {
                result = new MethodPolicy();
                context.Policies.Set<IMethodPolicy>(result, typeToBuild, idToBuild);
            }

            result.Methods.Add(member.Name, new MethodCallInfo(member.MemberInfo, parameters));
        }

        protected override IEnumerable<IReflectionMemberInfo<MethodInfo>> GetMembers(IBuilderContext context,
                                                                                     Type typeToBuild,
                                                                                     object existing,
                                                                                     string idToBuild)
        {
            foreach (MethodInfo method in typeToBuild.GetMethods())
                yield return new ReflectionMemberInfo<MethodInfo>(method);
        }

        protected override bool MemberRequiresProcessing(IReflectionMemberInfo<MethodInfo> member)
        {
            return (member.GetCustomAttributes(typeof(InjectionMethodAttribute), true).Length > 0);
        }
    }
}