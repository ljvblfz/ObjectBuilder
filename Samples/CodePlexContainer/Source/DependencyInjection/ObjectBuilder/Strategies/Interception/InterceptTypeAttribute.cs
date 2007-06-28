using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class InterceptTypeAttribute : Attribute
    {
        readonly InterceptionType interceptionType;

        public InterceptTypeAttribute(InterceptionType interceptionType)
        {
            this.interceptionType = interceptionType;
        }

        public InterceptionType InterceptionType
        {
            get { return interceptionType; }
        }
    }
}