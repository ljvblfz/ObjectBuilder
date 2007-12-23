using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public static class InterceptionReflector
    {
        public static void Reflect<TBeingBuilt>(IPolicyList policies,
                                                IObjectFactory factory)
        {
            Reflect(typeof(TBeingBuilt), typeof(TBeingBuilt), policies, factory);
        }

        public static void Reflect<TRequested, TBeingBuilt>(IPolicyList policies,
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

            foreach (KeyValuePair<Type, InterceptionPolicy> kvp in typePolicies)
                policyList.Set(kvp.Key, kvp.Value, typeBeingBuilt);
        }

        static void ReflectOnMethod(Type typeRequested,
                                    Type typeBeingBuilt,
                                    IDictionary<Type, InterceptionPolicy> typePolicies,
                                    MethodBase method,
                                    IObjectFactory factory)
        {
            Dictionary<KeyValuePair<Type, Type>, List<IInterceptionHandler>> methodHandlers = new Dictionary<KeyValuePair<Type, Type>, List<IInterceptionHandler>>();
            MethodBase methodForPolicy = method;

            foreach (InterceptAttribute attr in method.GetCustomAttributes(typeof(InterceptAttribute), true))
            {
                KeyValuePair<Type, Type> key = new KeyValuePair<Type, Type>(attr.PolicyInterfaceType, attr.PolicyConcreteType);

                if (!methodHandlers.ContainsKey(key))
                {
                    if (!typePolicies.ContainsKey(attr.PolicyInterfaceType))
                        attr.ValidateInterceptionForType(typeRequested, typeBeingBuilt);

                    attr.ValidateInterceptionForMethod(method);

                    methodForPolicy = attr.GetMethodBaseForPolicy(typeRequested, method);
                    if (methodForPolicy == null)
                        return;

                    methodHandlers[key] = new List<IInterceptionHandler>();
                }

                methodHandlers[key].Add((IInterceptionHandler)factory.Get(attr.HandlerType));
            }

            foreach (KeyValuePair<Type, Type> key in methodHandlers.Keys)
            {
                if (!typePolicies.ContainsKey(key.Key))
                    typePolicies[key.Key] = (InterceptionPolicy)factory.Get(key.Value);

                typePolicies[key.Key].Add(methodForPolicy, methodHandlers[key]);
            }
        }
    }
}