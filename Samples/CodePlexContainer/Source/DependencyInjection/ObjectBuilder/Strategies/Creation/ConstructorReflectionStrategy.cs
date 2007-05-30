using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ConstructorReflectionStrategy : ReflectionStrategy<ConstructorInfo>
    {
        protected override void AddParametersToPolicy(IBuilderContext context,
                                                      Type typeToBuild,
                                                      string idToBuild,
                                                      IReflectionMemberInfo<ConstructorInfo> member,
                                                      IEnumerable<IParameter> parameters)
        {
            ConstructorPolicy policy = new ConstructorPolicy();

            foreach (IParameter parameter in parameters)
                policy.AddParameter(parameter);

            context.Policies.Set<ICreationPolicy>(policy, typeToBuild, idToBuild);
        }

        protected override IEnumerable<IReflectionMemberInfo<ConstructorInfo>> GetMembers(IBuilderContext context,
                                                                                          Type typeToBuild,
                                                                                          object existing,
                                                                                          string idToBuild)
        {
            List<IReflectionMemberInfo<ConstructorInfo>> result = new List<IReflectionMemberInfo<ConstructorInfo>>();
            ICreationPolicy existingPolicy = context.Policies.Get<ICreationPolicy>(typeToBuild, idToBuild);

            if (existing == null && (existingPolicy == null || existingPolicy is DefaultCreationPolicy))
            {
                ConstructorInfo injectionCtor = null;
                ConstructorInfo[] ctors = typeToBuild.GetConstructors();

                if (ctors.Length == 1)
                    injectionCtor = ctors[0];
                else
                {
                    foreach (ConstructorInfo ctor in ctors)
                    {
                        if (Attribute.IsDefined(ctor, typeof(InjectionConstructorAttribute)))
                        {
                            // Multiple decorated constructors aren't valid
                            if (injectionCtor != null)
                                throw new InvalidAttributeException();

                            injectionCtor = ctor;
                        }
                    }
                }

                if (injectionCtor != null)
                    result.Add(new ReflectionMemberInfo<ConstructorInfo>(injectionCtor));
            }

            return result;
        }

        /// <summary>
        /// See <see cref="ReflectionStrategy{T}.MemberRequiresProcessing"/> for more information.
        /// </summary>
        protected override bool MemberRequiresProcessing(IReflectionMemberInfo<ConstructorInfo> member)
        {
            return true;
        }
    }
}