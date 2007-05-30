using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectionConstructorAttribute : Attribute {}
}