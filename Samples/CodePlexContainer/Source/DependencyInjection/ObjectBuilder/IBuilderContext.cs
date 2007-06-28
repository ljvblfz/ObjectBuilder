using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilderContext
    {
        IBuilderStrategy HeadOfChain { get; }
        ILifetimeContainer Lifetime { get; }
        IReadWriteLocator Locator { get; }
        string OriginalID { get; }
        Type OriginalType { get; }
        PolicyList Policies { get; }

        IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy);
    }
}