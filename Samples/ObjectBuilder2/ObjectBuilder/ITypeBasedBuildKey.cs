using System;

namespace ObjectBuilder
{
    public interface ITypeBasedBuildKey
    {
        Type Type { get; }
    }
}