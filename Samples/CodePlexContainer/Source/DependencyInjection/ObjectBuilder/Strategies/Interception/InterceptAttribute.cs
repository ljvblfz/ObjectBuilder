using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public class InterceptAttribute : Attribute
    {
        // Fields

        Type interceptionHandlerType;

        // Lifetime

        public InterceptAttribute(Type interceptionHandlerType)
        {
            this.interceptionHandlerType = interceptionHandlerType;
        }

        // Properties

        public Type InterceptionHandlerType
        {
            get { return interceptionHandlerType; }
        }
    }
}