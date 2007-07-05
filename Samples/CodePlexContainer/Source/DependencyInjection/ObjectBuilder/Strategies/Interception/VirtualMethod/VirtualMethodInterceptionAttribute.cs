using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodInterceptionAttribute : InterceptedClassAttribute
    {
        public override InterceptionPolicy CreatePolicy(Type typeRequested,
                                                        Type typeBeingBuilt)
        {
            if (!typeBeingBuilt.IsInterface && typeBeingBuilt.IsSealed)
                throw new InvalidOperationException("Type " + typeBeingBuilt.FullName + " is not compatible with virtual method interception");

            return new VirtualMethodInterceptionPolicy();
        }
    }
}