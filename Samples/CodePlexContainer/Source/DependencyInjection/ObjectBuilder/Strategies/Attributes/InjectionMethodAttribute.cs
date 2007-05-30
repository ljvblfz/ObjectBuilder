using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InjectionMethodAttribute : Attribute {}
}