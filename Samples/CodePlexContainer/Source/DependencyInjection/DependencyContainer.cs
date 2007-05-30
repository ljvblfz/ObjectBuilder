using System;
using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    public class DependencyContainer : IObjectFactory, IDisposable
    {
        // Fields

        IBuilder<BuilderStage> builder;
        ILifetimeContainer lifetime;
        IReadWriteLocator locator;
        PolicyList policies = new PolicyList();
        bool disposed;
        DependencyContainer innerContainer;

        // Lifetime

        public DependencyContainer()
        {
            disposed = false;
            locator = new Locator();
            lifetime = new LifetimeContainer();
            locator.Add(typeof(ILifetimeContainer), lifetime);
            builder = new BuilderBase<BuilderStage>();

            locator.Add(new DependencyResolutionLocatorKey(typeof(IObjectFactory), null), this);

            builder.Strategies.AddNew<LifetimeStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<TypeMappingStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<MethodReflectionStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<PropertyReflectionStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
            builder.Strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<MethodExecutionStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<BuilderAwareStrategy>(BuilderStage.PostInitialization);

            policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
        }

        public DependencyContainer(DependencyContainer innerContainer)
        {
            this.innerContainer = innerContainer;

            disposed = false;
            locator = new Locator(innerContainer.locator);
            lifetime = new LifetimeContainer();
            locator.Add(typeof(ILifetimeContainer), lifetime);
            builder = new BuilderBase<BuilderStage>();

            locator.Add(new DependencyResolutionLocatorKey(typeof(IObjectFactory), null), this);

            builder.Strategies.AddNew<LifetimeStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<TypeMappingStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<MethodReflectionStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<PropertyReflectionStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
            builder.Strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<MethodExecutionStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<BuilderAwareStrategy>(BuilderStage.PostInitialization);

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
            return builder.BuildUp(locator, typeToBuild, null, null, GetPoliciesForBuild());
        }

        PolicyList GetPoliciesForBuild()
        {
            PolicyList buildPolicies = new PolicyList();

            if (innerContainer != null)
                buildPolicies.AddPolicies(innerContainer.GetPoliciesForBuild());

            buildPolicies.AddPolicies(policies);
            return buildPolicies;
        }

        public void Inject(object existingObject)
        {
            builder.BuildUp(locator, existingObject.GetType(), null, existingObject);
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
            builder.TearDown(locator, existingObject);
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
                ILifetimeContainer lifetime = context.Locator.Get<ILifetimeContainer>();

                if (lifetime != null)
                    lifetime.Add(obj);

                return obj;
            }
        }
    }
}