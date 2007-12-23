using System;
using System.Collections.Generic;

namespace ObjectBuilder
{
    public interface IExtensibleObjectFactory : IObjectFactory
    {
        IPolicyList Policies { get; }

        void AddStrategy(IBuilderStrategy strategy,
                         BuilderStage stage);

        List<IObjectFactoryExtension> FindAllExtensions(Predicate<IObjectFactoryExtension> match);

        IObjectFactoryExtension FindExtension(Predicate<IObjectFactoryExtension> match);

        IBuilderStrategy FindStrategy(Predicate<IBuilderStrategy> match);
    }
}