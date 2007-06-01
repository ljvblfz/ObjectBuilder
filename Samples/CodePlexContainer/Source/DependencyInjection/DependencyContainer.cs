using System;
using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    public class DependencyContainer : IObjectFactory, IDisposable
    {
        // Fields

        Builder builder;
        ILifetimeContainer lifetime;
        IReadWriteLocator locator;
        PolicyList policies;
        StagedStrategyChain<BuilderStage> strategies = new StagedStrategyChain<BuilderStage>();
        bool disposed;

        // Lifetime

        public DependencyContainer()
            : this(null, null) {}

        public DependencyContainer(DependencyContainer innerContainer)
            : this(innerContainer.locator, innerContainer.policies) {}

        DependencyContainer(IReadWriteLocator innerLocator,
                            PolicyList innerPolicyList)
        {
            disposed = false;
            locator = new Locator(innerLocator);
            lifetime = new LifetimeContainer();
            builder = new Builder();
            policies = new PolicyList(innerPolicyList);

            locator.Add(new DependencyResolutionLocatorKey(typeof(IObjectFactory), null), this);

            strategies.AddNew<LifetimeStrategy>(BuilderStage.PreCreation);
            strategies.AddNew<TypeMappingStrategy>(BuilderStage.PreCreation);
            strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
            strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
            strategies.AddNew<MethodReflectionStrategy>(BuilderStage.PreCreation);
            strategies.AddNew<PropertyReflectionStrategy>(BuilderStage.PreCreation);
            strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
            strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);
            strategies.AddNew<MethodExecutionStrategy>(BuilderStage.Initialization);
            strategies.AddNew<BuilderAwareStrategy>(BuilderStage.PostInitialization);

            policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                lifetime.Dispose();
            }
        }

        // Methods

        public void CacheInstancesOf<T>()
        {
            policies.Set<ISingletonPolicy>(new SingletonPolicy(true), typeof(T), null);
        }

        public TToBuild Get<TToBuild>()
        {
            return (TToBuild)Get(typeof(TToBuild));
        }

        public object Get(Type typeToBuild)
        {
            return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), typeToBuild, null, null);
        }

        public void RegisterSingletonInstance(Type typeToRegisterAs,
                                              object instance)
        {
            if (!typeToRegisterAs.IsInstanceOfType(instance))
                throw new ArgumentException("Object is not type compatible with registration type", "instance");

            locator.Add(new DependencyResolutionLocatorKey(typeToRegisterAs, null), instance);
            lifetime.Add(instance);
        }

        public void RegisterSingletonInstance<TTypeToRegisterAs>(TTypeToRegisterAs instance)
        {
            RegisterSingletonInstance(typeof(TTypeToRegisterAs), instance);
        }

        public void RegisterTypeMapping<TRequested, TToBuild>()
        {
            RegisterTypeMapping(typeof(TRequested), typeof(TToBuild));
        }

        public void RegisterTypeMapping(Type typeRequested,
                                        Type typeToBuild)
        {
            policies.Set<ITypeMappingPolicy>(new TypeMappingPolicy(typeToBuild, null), typeRequested, null);
        }

        public void TearDown(object existingObject)
        {
            builder.TearDown(locator, lifetime, policies, strategies.MakeStrategyChain(), existingObject);
        }

        // Inner types

        class LifetimeStrategy : BuilderStrategy
        {
            public override object BuildUp(IBuilderContext context,
                                           Type typeToBuild,
                                           object existing,
                                           string idToBuild)
            {
                object obj = base.BuildUp(context, typeToBuild, existing, idToBuild);

                if (context.Lifetime != null)
                    context.Lifetime.Add(obj);

                return obj;
            }
        }
    }
}