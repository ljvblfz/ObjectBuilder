using System;

namespace ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RemotingInterceptAttribute : InterceptAttribute
    {
        public RemotingInterceptAttribute(Type handlerType)
            : base(handlerType) {}

        public override Type PolicyConcreteType
        {
            get { return typeof(RemotingInterceptionPolicy); }
        }

        public override Type PolicyInterfaceType
        {
            get { return typeof(IRemotingInterceptionPolicy); }
        }

        public override void ValidateInterceptionForType(Type typeRequested,
                                                         Type typeBeingBuilt)
        {
            if (!typeof(MarshalByRefObject).IsAssignableFrom(typeBeingBuilt))
                throw new InvalidOperationException("Type " + typeBeingBuilt.FullName + " must derive from MarshalByRefObject to be intercepted.");
        }
    }
}