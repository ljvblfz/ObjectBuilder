using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class InterceptAttribute : Attribute
    {
        readonly Type handlerType;

        public InterceptAttribute(Type handlerType)
        {
            this.handlerType = handlerType;
        }

        public Type HandlerType
        {
            get { return handlerType; }
        }

        public abstract Type PolicyType { get; }

        public virtual bool ShouldInterceptMethod(Type typeRequested, MethodBase method)
        {
            return true;
        }

        public virtual void ValidateInterceptionForMethod(MethodBase method)
        {
        }

        public virtual void ValidateInterceptionForType(Type typeRequested,
                                                         Type typeBeingBuilt)
        {
        }
    }
}