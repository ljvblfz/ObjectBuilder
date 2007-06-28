using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class RemotingProxy : RealProxy, IRemotingTypeInfo
    {
        // Fields

        readonly Dictionary<MethodBase, HandlerPipeline> handlers;
        readonly object target;
        readonly Type typeOfTarget;

        // Lifetime

        public RemotingProxy(object target,
                             Type typeToProxy,
                             IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
            : base(typeToProxy)
        {
            this.target = target;
            this.handlers = new Dictionary<MethodBase, HandlerPipeline>();

            foreach (KeyValuePair<MethodBase, List<IInterceptionHandler>> kvp in handlers)
                this.handlers.Add(kvp.Key, new HandlerPipeline(kvp.Value));

            typeOfTarget = target.GetType();
        }

        // Properties

        public object Target
        {
            get { return target; }
        }

        string IRemotingTypeInfo.TypeName
        {
            get { return typeOfTarget.FullName; }
            set { throw new NotImplementedException(); }
        }

        // Methods

        bool IRemotingTypeInfo.CanCastTo(Type fromType,
                                         object obj)
        {
            return fromType.IsAssignableFrom(obj.GetType());
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMessage = (IMethodCallMessage)msg;
            HandlerPipeline pipeline;
            MethodBase method = callMessage.MethodBase;

            if (method.ReflectedType != typeOfTarget)
            {
                List<Type> paramTypes = new List<Type>();

                foreach (ParameterInfo paramInfo in method.GetParameters())
                    paramTypes.Add(paramInfo.ParameterType);

                method = typeOfTarget.GetMethod(method.Name,
                                                BindingFlags.Public | BindingFlags.Instance,
                                                null,
                                                method.CallingConvention,
                                                paramTypes.ToArray(),
                                                null);
            }

            if (handlers.ContainsKey(callMessage.MethodBase))
                pipeline = handlers[callMessage.MethodBase];
            else if (handlers.ContainsKey(method))
                pipeline = handlers[method];
            else
                pipeline = new HandlerPipeline();

            MethodInvocation invocation = new MethodInvocation(target, callMessage.MethodBase, callMessage.Args);

            IMethodReturn result =
                pipeline.Invoke(invocation,
                                delegate
                                {
                                    try
                                    {
                                        object returnValue = callMessage.MethodBase.Invoke(target, invocation.Arguments);
                                        return new MethodReturn(invocation.Arguments, callMessage.MethodBase.GetParameters(), returnValue);
                                    }
                                    catch (TargetInvocationException ex)
                                    {
                                        return new MethodReturn(ex.InnerException, callMessage.MethodBase.GetParameters());
                                    }
                                });

            if (result.Exception == null)
                return new ReturnMessage(result.ReturnValue, invocation.Arguments, invocation.Arguments.Length,
                                         callMessage.LogicalCallContext, callMessage);

            return new ReturnMessage(result.Exception, callMessage);
        }
    }
}