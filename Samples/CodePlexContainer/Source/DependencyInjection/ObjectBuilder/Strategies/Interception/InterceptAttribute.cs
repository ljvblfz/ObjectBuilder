using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true, Inherited=true)]
    public class InterceptAttribute : Attribute
    {
        readonly Type interceptionHandlerType;

        public InterceptAttribute(Type interceptionHandlerType)
        {
            this.interceptionHandlerType = interceptionHandlerType;
        }

        public Type InterceptionHandlerType
        {
            get { return interceptionHandlerType; }
        }
    }
}