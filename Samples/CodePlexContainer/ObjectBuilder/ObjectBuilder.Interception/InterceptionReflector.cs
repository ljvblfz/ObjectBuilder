using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public static class InterceptionReflector
    {
        public static void Reflect<TBeingBuilt>(PolicyList policies,
                                                IObjectFactory factory)
        {
            Reflect(typeof(TBeingBuilt), typeof(TBeingBuilt), policies, factory);
        }

        public static void Reflect<TRequested, TBeingBuilt>(PolicyList policies,
                                                            IObjectFactory factory)
            where TBeingBuilt : TRequested
        {
            Reflect(typeof(TRequested), typeof(TBeingBuilt), policies, factory);
        }

        public static void Reflect(Type typeRequested,
                                   Type typeBeingBuilt,
                                   IPolicyList policyList,
                                   IObjectFactory factory)
        {
            if (typeRequested.IsGenericType && typeBeingBuilt.IsGenericType)
            {
                typeRequested = typeRequested.GetGenericTypeDefinition();
                typeBeingBuilt = typeBeingBuilt.GetGenericTypeDefinition();
            }

            Dictionary<Type, InterceptionPolicy> typePolicies = new Dictionary<Type, InterceptionPolicy>();

            foreach (MethodInfo method in typeBeingBuilt.GetMethods())
                ReflectOnMethod(typeRequested, typeBeingBuilt, typePolicies, method, factory);

            foreach (InterceptionPolicy policy in typePolicies.Values)
                policyList.Set(policy.GetType(), policy, typeBeingBuilt);
        }

        static void ReflectOnMethod(Type typeRequested,
                                    Type typeBeingBuilt,
                                    IDictionary<Type, InterceptionPolicy> typePolicies,
                                    MethodBase method,
                                    IObjectFactory factory)
        {
            Dictionary<Type, List<IInterceptionHandler>> methodHandlers = new Dictionary<Type, List<IInterceptionHandler>>();
            MethodBase methodForPolicy = method;

            foreach (InterceptAttribute attr in method.GetCustomAttributes(typeof(InterceptAttribute), true))
            {
                if (!methodHandlers.ContainsKey(attr.PolicyType))
                {
                    if (!typePolicies.ContainsKey(attr.PolicyType))
                        attr.ValidateInterceptionForType(typeRequested, typeBeingBuilt);

                    attr.ValidateInterceptionForMethod(method);

                    methodForPolicy = attr.GetMethodBaseForPolicy(typeRequested, method);
                    if (methodForPolicy == null)
                        return;

                    methodHandlers[attr.PolicyType] = new List<IInterceptionHandler>();
                }

                methodHandlers[attr.PolicyType].Add((IInterceptionHandler)factory.Get(attr.HandlerType));
            }

            foreach (Type policyType in methodHandlers.Keys)
            {
                if (!typePolicies.ContainsKey(policyType))
                    typePolicies[policyType] = (InterceptionPolicy)factory.Get(policyType);

                typePolicies[policyType].Add(methodForPolicy, methodHandlers[policyType]);
            }
        }
    }
}