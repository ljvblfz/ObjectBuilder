using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public static class InterceptionReflector
    {
        public static IInterceptionPolicy Reflect<TBeingBuilt>(IObjectFactory factory)
        {
            return Reflect(typeof(TBeingBuilt), typeof(TBeingBuilt), factory);
        }

        public static IInterceptionPolicy Reflect<TRequested, TBeingBuilt>(IObjectFactory factory)
            where TBeingBuilt : TRequested
        {
            return Reflect(typeof(TRequested), typeof(TBeingBuilt), factory);
        }

        public static IInterceptionPolicy Reflect(Type typeRequested,
                                                  Type typeBeingBuilt,
                                                  IObjectFactory objectFactory)
        {
            foreach (InterceptTypeAttribute interceptTypeAttribute in typeBeingBuilt.GetCustomAttributes(typeof(InterceptTypeAttribute), true))
            {
                InterceptionPolicy policy = new InterceptionPolicy(interceptTypeAttribute.InterceptionType);

                if (interceptTypeAttribute.InterceptionType == InterceptionType.Remoting)
                    if (!typeRequested.IsInterface && !typeof(MarshalByRefObject).IsAssignableFrom(typeRequested))
                        throw new ArgumentException("Type " + typeRequested.FullName + " is not compatible with remoting", "typeBeingBuilt");

                foreach (MethodInfo method in typeBeingBuilt.GetMethods())
                {
                    List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();

                    foreach (InterceptAttribute interceptAttr in method.GetCustomAttributes(typeof(InterceptAttribute), true))
                        handlers.Add((IInterceptionHandler)objectFactory.Get(interceptAttr.InterceptionHandlerType));

                    if (handlers.Count > 0)
                        policy.Add(method, handlers);
                }

                if (policy.Count > 0)
                    return policy;
            }

            return null;
        }
    }
}