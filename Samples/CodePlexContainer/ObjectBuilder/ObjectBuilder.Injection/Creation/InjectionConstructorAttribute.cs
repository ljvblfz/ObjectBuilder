using System;

namespace ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectionConstructorAttribute : Attribute {}
}