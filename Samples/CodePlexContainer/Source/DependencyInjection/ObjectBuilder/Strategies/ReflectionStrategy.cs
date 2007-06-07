using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class ReflectionStrategy<TMemberInfo> : BuilderStrategy
    {
        // Methods

        protected abstract void AddParametersToPolicy(IBuilderContext context,
                                                      Type typeToBuild,
                                                      string idToBuild,
                                                      IReflectionMemberInfo<TMemberInfo> member,
                                                      IEnumerable<IParameter> parameters);

        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            foreach (IReflectionMemberInfo<TMemberInfo> member in GetMembers(context, typeToBuild, existing, idToBuild))
            {
                if (MemberRequiresProcessing(member))
                {
                    IEnumerable<IParameter> parameters = GenerateIParametersFromParameterInfos(member.GetParameters());
                    AddParametersToPolicy(context, typeToBuild, idToBuild, member, parameters);
                }
            }

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        static IEnumerable<IParameter> GenerateIParametersFromParameterInfos(ParameterInfo[] parameterInfos)
        {
            List<IParameter> result = new List<IParameter>();

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                ParameterAttribute attribute = GetInjectionAttribute(parameterInfo);
                result.Add(attribute.CreateParameter(parameterInfo.ParameterType));
            }

            return result;
        }

        protected abstract IEnumerable<IReflectionMemberInfo<TMemberInfo>> GetMembers(IBuilderContext context,
                                                                                      Type typeToBuild,
                                                                                      object existing,
                                                                                      string idToBuild);

        static ParameterAttribute GetInjectionAttribute(ParameterInfo parameterInfo)
        {
            ParameterAttribute[] attributes = (ParameterAttribute[])parameterInfo.GetCustomAttributes(typeof(ParameterAttribute), true);

            switch (attributes.Length)
            {
                case 0:
                    return new DependencyAttribute();

                case 1:
                    return attributes[0];

                default:
                    throw new InvalidAttributeException();
            }
        }

        protected abstract bool MemberRequiresProcessing(IReflectionMemberInfo<TMemberInfo> member);
    }
}