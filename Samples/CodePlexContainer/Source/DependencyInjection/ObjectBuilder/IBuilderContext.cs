using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilderContext
    {
        // Properties

        IBuilderStrategy HeadOfChain { get; }
        ILifetimeContainer Lifetime { get; }
        IReadWriteLocator Locator { get; }
        string OriginalID { get; }
        Type OriginalType { get; }
        PolicyList Policies { get; }

        // Methods

        IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy);
    }
}