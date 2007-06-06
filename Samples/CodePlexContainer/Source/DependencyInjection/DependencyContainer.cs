using System;
using System.Reflection;
using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    public class DependencyContainer : IObjectFactory, IDisposable
    {
        // Fields

        bool disposed = false;
        Builder builder = new Builder();
        LifetimeContainer lifetime = new LifetimeContainer();
        Locator locator;
        PolicyList policies;
        StagedStrategyChain<BuilderStage> strategies;

        // Lifetime

        public DependencyContainer()
            : this(null, null, null) {}

        public DependencyContainer(DependencyContainer innerContainer)
            : this(innerContainer.locator, innerContainer.policies, innerContainer.strategies) {}

        DependencyContainer(Locator innerLocator,
                            PolicyList innerPolicies,
                            StagedStrategyChain<BuilderStage> innerStrategies)
        {
            locator = new Locator(innerLocator);
            policies = new PolicyList(innerPolicies);
            strategies = new StagedStrategyChain<BuilderStage>(innerStrategies);

            RegisterSingletonInstance<IObjectFactory>(this);

            if (innerStrategies == null)
            {
                strategies.AddNew<LifetimeStrategy>(BuilderStage.PreCreation);
                strategies.AddNew<TypeMappingStrategy>(BuilderStage.PreCreation);
                strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
                strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
                strategies.AddNew<MethodReflectionStrategy>(BuilderStage.PreCreation);
                strategies.AddNew<PropertyReflectionStrategy>(BuilderStage.PreCreation);
                strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
                strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);
                strategies.AddNew<MethodExecutionStrategy>(BuilderStage.Initialization);
                strategies.AddNew<RemotingInterceptionStrategy>(BuilderStage.PostInitialization);
                strategies.AddNew<BuilderAwareStrategy>(BuilderStage.PostInitialization);
            }

            if (innerPolicies == null)
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
            CacheInstancesOf(typeof(T));
        }

        public void CacheInstancesOf(Type typeToCache)
        {
            policies.Set<ISingletonPolicy>(new SingletonPolicy(true), typeToCache, null);
        }

        public TToBuild Get<TToBuild>()
        {
            return (TToBuild)Get(typeof(TToBuild));
        }

        public object Get(Type typeToBuild)
        {
            return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), typeToBuild, null, null);
        }

        public object Inject(object @object)
        {
            Guard.ArgumentNotNull(@object, "object");

            return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), @object.GetType(), null, @object);
        }

        public void Intercept<T>(MethodInfo method,
                                 params ICallHandler[] handlers)
        {
            Intercept(typeof(T), method, handlers);
        }

        public void Intercept(Type typeToIntercept,
                              MethodInfo method,
                              params ICallHandler[] handlers)
        {
            InterceptionPolicy policy = (InterceptionPolicy)policies.GetLocal<IInterceptionPolicy>(typeToIntercept, null);

            if (policy == null)
                throw new InvalidOperationException("Must call SetInterceptionType before calling Intercept");

            policy.Add(method, handlers);
        }

        public void RegisterSingletonInstance<TTypeToRegisterAs>(TTypeToRegisterAs instance)
        {
            RegisterSingletonInstance(typeof(TTypeToRegisterAs), instance);
        }

        public void RegisterSingletonInstance(Type typeToRegisterAs,
                                              object instance)
        {
            if (!typeToRegisterAs.IsInstanceOfType(instance))
                throw new ArgumentException("Object is not type compatible with registration type", "instance");

            locator.Add(new DependencyResolutionLocatorKey(typeToRegisterAs, null), instance);
            lifetime.Add(instance);
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

        public void SetInterceptionType<T>(InterceptionType interceptionType)
        {
            SetInterceptionType(typeof(T), interceptionType);
        }

        public void SetInterceptionType(Type typeToIntercept,
                                        InterceptionType interceptionType)
        {
            IInterceptionPolicy policy = policies.GetLocal<IInterceptionPolicy>(typeToIntercept, null);

            if (policy == null)
                policies.Set<IInterceptionPolicy>(new InterceptionPolicy(interceptionType), typeToIntercept, null);
            else if (policy.InterceptionType != interceptionType)
                throw new ArgumentException("Called SetInterceptionType when a conflicting interception type has already been requested", "interceptionType");
        }

        public void TearDown(object existingObject)
        {
            builder.TearDown(locator, lifetime, policies, strategies.MakeStrategyChain(), existingObject);
        }
    }
}