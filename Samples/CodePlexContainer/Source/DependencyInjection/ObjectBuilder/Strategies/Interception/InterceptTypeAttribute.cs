using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class InterceptTypeAttribute : Attribute
    {
        // Fields

        InterceptionType interceptionType;

        // Lifetime

        public InterceptTypeAttribute(InterceptionType interceptionType)
        {
            this.interceptionType = interceptionType;
        }

        // Properties

        public InterceptionType InterceptionType
        {
            get { return interceptionType; }
        }
    }
}