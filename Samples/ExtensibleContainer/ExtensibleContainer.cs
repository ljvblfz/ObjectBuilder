using System;
using System.Collections.Generic;

namespace ObjectBuilder
{
    public class ExtensibleContainer : IObjectFactory
    {
        readonly ObjectFactory innerFactory = new ObjectFactory();

        public virtual void Extend(IObjectFactoryExtension extension)
        {
            innerFactory.extensions.Add(extension);

            extension.Extend(innerFactory);
        }

        public List<IObjectFactoryExtension> FindAllExtensions(Predicate<IObjectFactoryExtension> match)
        {
            return innerFactory.FindAllExtensions(match);
        }

        public IObjectFactoryExtension FindExtension(Predicate<IObjectFactoryExtension> match)
        {
            return innerFactory.FindExtension(match);
        }

        public TExtension FindExtension<TExtension>()
            where TExtension : IObjectFactoryExtension
        {
            return (TExtension)innerFactory.FindExtension(
                                   delegate(IObjectFactoryExtension extension)
                                   {
                                       return typeof(TExtension).IsAssignableFrom(extension.GetType());
                                   });
        }

        public object Get(Type type)
        {
            return innerFactory.Get(type);
        }

        public object Get(string name)
        {
            return innerFactory.Get(name);
        }

        public T Get<T>()
        {
            return innerFactory.Get<T>();
        }

        public T Get<T>(string name)
        {
            return innerFactory.Get<T>(name);
        }

        public object Inject(object @object)
        {
            return innerFactory.Inject(@object);
        }

        class ObjectFactory : IExtensibleObjectFactory
        {
            public readonly Builder builder = new Builder();
            public readonly List<IObjectFactoryExtension> extensions = new List<IObjectFactoryExtension>();
            public readonly LifetimeContainer lifetime = new LifetimeContainer();
            public readonly Locator locator = new Locator();
            public readonly IPolicyList policies = new PolicyList();
            public readonly StagedStrategyChain<BuilderStage> strategies = new StagedStrategyChain<BuilderStage>();

            public IPolicyList Policies
            {
                get { return policies; }
            }

            public void AddStrategy(IBuilderStrategy strategy,
                                    BuilderStage stage)
            {
                strategies.Add(strategy, stage);
            }

            public List<IObjectFactoryExtension> FindAllExtensions(Predicate<IObjectFactoryExtension> match)
            {
                return extensions.FindAll(match);
            }

            public IObjectFactoryExtension FindExtension(Predicate<IObjectFactoryExtension> match)
            {
                return extensions.Find(match);
            }

            public IBuilderStrategy FindStrategy(Predicate<IBuilderStrategy> match)
            {
                // TODO: This is pretty non-optimal; chains should be enumerable, even staged chains

                StrategyChain chain = strategies.MakeStrategyChain();
                IBuilderStrategy strategy = chain.Head;

                while (strategy != null)
                {
                    if (match(strategy))
                        return strategy;

                    strategy = chain.GetNext(strategy);
                }

                return null;
            }

            public object Get(Type type)
            {
                return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), type, null);
            }

            public object Get(string name)
            {
                return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), name, null);
            }

            public T Get<T>()
            {
                return builder.BuildUp<T>(locator, lifetime, policies, strategies.MakeStrategyChain(), typeof(T), null);
            }

            public T Get<T>(string name)
            {
                return builder.BuildUp<T>(locator, lifetime, policies, strategies.MakeStrategyChain(), name, null);
            }

            public object Inject(object @object)
            {
                return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), @object.GetType(), @object);
            }
        }
    }
}