using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RemotingInterceptAttribute : InterceptAttribute
    {
        public RemotingInterceptAttribute(Type handlerType)
            : base(handlerType) {}

        public override Type PolicyType
        {
            get { return typeof(RemotingInterceptionPolicy); }
        }

        public override void ValidateInterceptionForType(Type typeRequested,
                                                  Type typeBeingBuilt)
        {
            if (!typeRequested.IsInterface &&
                !typeof(MarshalByRefObject).IsAssignableFrom(typeBeingBuilt))
                throw new InvalidOperationException("Type " + typeBeingBuilt.FullName + " is not compatible with remoting interception");
        }
    }
}