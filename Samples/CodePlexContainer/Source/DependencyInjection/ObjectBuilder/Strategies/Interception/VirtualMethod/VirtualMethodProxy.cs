using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodProxy
    {
        readonly Dictionary<MethodBase, HandlerPipeline> handlers;

        public VirtualMethodProxy(IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
        {
            this.handlers = new Dictionary<MethodBase, HandlerPipeline>();

            foreach (KeyValuePair<MethodBase, List<IInterceptionHandler>> kvp in handlers)
                this.handlers.Add(kvp.Key, new HandlerPipeline(kvp.Value));
        }

        public object Invoke(object target,
                             MethodBase method,
                             object[] arguments)
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
                                        object returnValue = method.Invoke(target, invocation.Arguments);
                                        return new MethodReturn(invocation.Arguments, method.GetParameters(), returnValue);
                                    }
                                    catch (TargetInvocationException ex)
                                    {
                                        return new MethodReturn(ex.InnerException, method.GetParameters());
                                    }
                                });

            if (result.Exception != null)
                throw result.Exception; // UGH! Lost the stack trace. What can we do?

            return result.ReturnValue;
        }
    }
}