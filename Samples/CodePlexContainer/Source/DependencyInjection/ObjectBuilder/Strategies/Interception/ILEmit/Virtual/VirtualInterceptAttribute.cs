using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class VirtualInterceptAttribute : InterceptAttribute
    {
        public VirtualInterceptAttribute(Type handlerType)
            : base(handlerType) {}

        public override Type PolicyType
        {
            get { return typeof(VirtualInterceptionPolicy); }
        }

        public override void ValidateInterceptionForMethod(MethodBase method)
        {
            if (!method.IsVirtual || method.IsFinal)
                throw new InvalidOperationException("Method " + method.DeclaringType.FullName + "." + method.Name + " must be virtual and not sealed to be intercepted.");
        }

        public override void ValidateInterceptionForType(Type typeRequested,
                                                         Type typeBeingBuilt)
        {
            if ((!typeBeingBuilt.IsPublic && !typeBeingBuilt.IsNestedPublic) || typeBeingBuilt.IsSealed)
                throw new InvalidOperationException("Type " + typeBeingBuilt.FullName + " must be public and not sealed.");
        }
    }
}