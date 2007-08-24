using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ConstructorReflectionStrategy : ReflectionStrategy<ConstructorInfo>
    {
        protected override void AddParametersToPolicy(IBuilderContext context,
                                                      object buildKey,
                                                      IMemberInfo<ConstructorInfo> member,
                                                      IEnumerable<IParameter> parameters)
        {
            ConstructorCreationPolicy policy = new ConstructorCreationPolicy(member.MemberInfo, parameters);
            context.Policies.Set<ICreationPolicy>(policy, buildKey);
        }

        protected override IEnumerable<IMemberInfo<ConstructorInfo>> GetMembers(IBuilderContext context,
                                                                                object buildKey,
                                                                                object existing)
        {
            ICreationPolicy existingPolicy = context.Policies.GetNoDefault<ICreationPolicy>(buildKey, false);

            if (existing == null && existingPolicy == null)
            {
                Type typeToBuild = GetTypeFromBuildKey(buildKey);
                ConstructorInfo injectionCtor = null;
                ConstructorInfo[] ctors = typeToBuild.GetConstructors();

                if (ctors.Length == 1)
                    injectionCtor = ctors[0];
                else
                    foreach (ConstructorInfo ctor in ctors)
                        if (Attribute.IsDefined(ctor, typeof(InjectionConstructorAttribute)))
                        {
                            if (injectionCtor != null)
                                throw new InvalidAttributeException(typeToBuild, ".ctor");

                            injectionCtor = ctor;
                        }

                if (injectionCtor != null)
                    yield return new MethodMemberInfo<ConstructorInfo>(injectionCtor);
            }
        }

        protected override bool MemberRequiresProcessing(IMemberInfo<ConstructorInfo> member)
        {
            return true;
        }
    }
}