using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public abstract class ReflectionStrategy<TMemberInfo> : BuilderStrategy
    {
        protected abstract void AddParametersToPolicy(IBuilderContext context,
                                                      object buildKey,
                                                      IMemberInfo<TMemberInfo> member,
                                                      IEnumerable<IParameter> parameters);

        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            Type type;
            TryGetTypeFromBuildKey(buildKey, out type);

            foreach (IMemberInfo<TMemberInfo> member in GetMembers(context, buildKey, existing))
            {
                if (MemberRequiresProcessing(member))
                {
                    IEnumerable<IParameter> parameters =
                        GenerateIParametersFromParameterInfos(member.GetParameters(), type, member.Name);

                    AddParametersToPolicy(context, buildKey, member, parameters);
                }
            }

            return base.BuildUp(context, buildKey, existing);
        }

        static IEnumerable<IParameter> GenerateIParametersFromParameterInfos(IEnumerable<ParameterInfo> parameterInfos,
                                                                             Type type,
                                                                             string memberName)
        {
            List<IParameter> result = new List<IParameter>();

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                ParameterAttribute attribute = GetInjectionAttribute(parameterInfo, type, memberName);
                result.Add(attribute.CreateParameter(parameterInfo.ParameterType));
            }

            return result;
        }

        static ParameterAttribute GetInjectionAttribute(ICustomAttributeProvider parameterInfo,
                                                        Type type,
                                                        string memberName)
        {
            ParameterAttribute[] attributes = (ParameterAttribute[])parameterInfo.GetCustomAttributes(typeof(ParameterAttribute), true);

            switch (attributes.Length)
            {
                case 0:
                    return new DependencyAttribute();

                case 1:
                    return attributes[0];

                default:
                    throw new InvalidAttributeException(type, memberName);
            }
        }

        protected abstract IEnumerable<IMemberInfo<TMemberInfo>> GetMembers(IBuilderContext context,
                                                                            object buildKey,
                                                                            object existing);

        protected abstract bool MemberRequiresProcessing(IMemberInfo<TMemberInfo> member);
    }
}