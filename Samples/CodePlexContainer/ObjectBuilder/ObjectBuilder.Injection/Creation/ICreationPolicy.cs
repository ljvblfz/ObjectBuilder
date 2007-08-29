using System.Reflection;

namespace ObjectBuilder
{
    public interface ICreationPolicy : IBuilderPolicy
    {
        bool SupportsReflection { get; }

        object Create(IBuilderContext context,
                      object buildKey);

        ConstructorInfo GetConstructor(IBuilderContext context,
                                       object buildKey);

        object[] GetParameters(IBuilderContext context,
                               ConstructorInfo constructor);
    }
}