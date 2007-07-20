using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ILEmitProxy
    {
        public delegate object InvokeDelegate(object[] arguments);

        readonly Dictionary<MethodBase, HandlerPipeline> handlers;

        public ILEmitProxy(IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
        {
            this.handlers = new Dictionary<MethodBase, HandlerPipeline>();

            foreach (KeyValuePair<MethodBase, List<IInterceptionHandler>> kvp in handlers)
                this.handlers.Add(kvp.Key, new HandlerPipeline(kvp.Value));
        }

        HandlerPipeline GetPipeline(MethodInfo method,
                                    object target)
        {
            // Non-generic method
            if (handlers.ContainsKey(method))
                return handlers[method];

            // Generic method
            if (method.IsGenericMethod && handlers.ContainsKey(method.GetGenericMethodDefinition()))
                return handlers[method.GetGenericMethodDefinition()];

            // Non-generic method on generic type
            if (target.GetType().IsGenericType)
            {
                Type genericTarget = target.GetType().BaseType.GetGenericTypeDefinition();
                MethodInfo methodToLookup = genericTarget.GetMethod(method.Name);

                if (handlers.ContainsKey(methodToLookup))
                    return handlers[methodToLookup];
            }

            // Empty pipeline as a last resort
            return new HandlerPipeline();
        }

        public object Invoke(object target,
                             MethodInfo method,
                             object[] arguments,
                             InvokeDelegate @delegate)
        {
            HandlerPipeline pipeline = GetPipeline(method, target);
            MethodInvocation invocation = new MethodInvocation(target, method, arguments);

            IMethodReturn result =
                pipeline.Invoke(invocation,
                                delegate
                                {
                                    try
                                    {
                                        object returnValue = @delegate(arguments);
                                        return new MethodReturn(invocation.Arguments, method.GetParameters(), returnValue);
                                    }
                                    catch (Exception ex)
                                    {
                                        return new MethodReturn(ex, method.GetParameters());
                                    }
                                });

            if (result.Exception != null)
            {
                FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
                remoteStackTraceString.SetValue(result.Exception, result.Exception.StackTrace + Environment.NewLine);
                throw result.Exception;
            }

            return result.ReturnValue;
        }
    }
}