using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodProxy
    {
        public delegate object InvokeDelegate(object[] arguments);

        readonly Dictionary<MethodBase, HandlerPipeline> handlers;

        public VirtualMethodProxy(IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
        {
            this.handlers = new Dictionary<MethodBase, HandlerPipeline>();

            foreach (KeyValuePair<MethodBase, List<IInterceptionHandler>> kvp in handlers)
                this.handlers.Add(kvp.Key, new HandlerPipeline(kvp.Value));
        }

        public object Invoke(object target,
                             MethodBase method,
                             object[] arguments,
                             InvokeDelegate @delegate)
        {
            HandlerPipeline pipeline;

            if (handlers.ContainsKey(method))
                pipeline = handlers[method];
            else
                pipeline = new HandlerPipeline();

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