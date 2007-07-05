using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class RemotingInterceptionAttribute : InterceptedClassAttribute
    {
        public override InterceptionPolicy CreatePolicy(Type typeRequested,
                                                        Type typeBeingBuilt)
        {
            if (!typeRequested.IsInterface &&
                !typeof(MarshalByRefObject).IsAssignableFrom(typeBeingBuilt))
                throw new InvalidOperationException("Type " + typeBeingBuilt.FullName + " is not compatible with remoting interception");

            return new RemotingInterceptionPolicy();
        }
    }
}