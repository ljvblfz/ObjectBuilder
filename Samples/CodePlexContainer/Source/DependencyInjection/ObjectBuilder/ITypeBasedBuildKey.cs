using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface ITypeBasedBuildKey
    {
        Type Type { get; }
    }
}