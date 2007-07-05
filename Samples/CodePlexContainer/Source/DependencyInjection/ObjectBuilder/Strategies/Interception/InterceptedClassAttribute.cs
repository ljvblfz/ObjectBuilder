using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public abstract class InterceptedClassAttribute : Attribute
    {
        public abstract InterceptionPolicy CreatePolicy(Type typeRequested,
                                                        Type typeBeingBuilt);
    }
}