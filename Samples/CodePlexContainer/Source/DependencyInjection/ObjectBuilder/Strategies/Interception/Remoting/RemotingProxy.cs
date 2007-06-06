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

        Dictionary<MethodBase, HandlerPipeline> handlers;
        object target;
        string typeName;

        // Lifetime

        public RemotingProxy(object target,
                             Type classToProxy,
                             IEnumerable<KeyValuePair<MethodBase, List<ICallHandler>>> handlers)
            : base(classToProxy)
        {
            this.target = target;
            this.handlers = new Dictionary<MethodBase, HandlerPipeline>();

            foreach (KeyValuePair<MethodBase, List<ICallHandler>> kvp in handlers)
                this.handlers.Add(kvp.Key, new HandlerPipeline(kvp.Value));

            typeName = target.GetType().FullName;
        }

        // Properties

        public object Target
        {
            get { return target; }
        }

        string IRemotingTypeInfo.TypeName
        {
            get { return typeName; }
            set { typeName = value; }
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

            if (handlers.ContainsKey(callMessage.MethodBase))
                pipeline = handlers[callMessage.MethodBase];
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