using System;
using System.Reflection;
using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    public class DependencyContainer : IObjectFactory, IDisposable
    {
        readonly Builder builder = new Builder();
        bool disposed = false;
        readonly LifetimeContainer lifetime = new LifetimeContainer();
        readonly Locator locator;
        readonly PolicyList policies;
        readonly StagedStrategyChain<BuilderStage> strategies;

        public DependencyContainer()
            : this(null, null, null, null) {}

        public DependencyContainer(IDependencyContainerConfigurator configurator)
            : this(null, null, null, configurator) {}

        public DependencyContainer(DependencyContainer innerContainer)
            : this(innerContainer.locator, innerContainer.policies, innerContainer.strategies, null) {}

        public DependencyContainer(DependencyContainer innerContainer,
                                   IDependencyContainerConfigurator configurator)
            : this(innerContainer.locator, innerContainer.policies, innerContainer.strategies, configurator) {}

        DependencyContainer(IReadableLocator innerLocator,
                            IPolicyList innerPolicies,
                            StagedStrategyChain<BuilderStage> innerStrategies,
                            IDependencyContainerConfigurator configurator)
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
                strategies.AddNew<EventBrokerReflectionStrategy>(BuilderStage.PreCreation);
                strategies.AddNew<InterceptionReflectionStrategy>(BuilderStage.PreCreation);

                strategies.AddNew<InterfaceInterceptionStrategy>(BuilderStage.Creation);
                strategies.AddNew<VirtualInterceptionStrategy>(BuilderStage.Creation);
                strategies.AddNew<CreationStrategy>(BuilderStage.Creation);

                strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);
                strategies.AddNew<MethodExecutionStrategy>(BuilderStage.Initialization);
                strategies.AddNew<EventBrokerStrategy>(BuilderStage.Initialization);

                strategies.AddNew<BuilderAwareStrategy>(BuilderStage.PostInitialization);
                strategies.AddNew<RemotingInterceptionStrategy>(BuilderStage.PostInitialization);
            }

            if (innerPolicies == null)
                policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());

            locator.Add(typeof(EventBrokerService), new EventBrokerService());

            if (configurator != null)
                configurator.Configure(this);
        }

        public void CacheInstancesOf<T>()
        {
            CacheInstancesOf(typeof(T));
        }

        public void CacheInstancesOf(Type typeToCache)
        {
            policies.Set<ISingletonPolicy>(new SingletonPolicy(true), typeToCache, null);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                lifetime.Dispose();
            }
        }

        public TToBuild Get<TToBuild>()
        {
            return (TToBuild)Get(typeof(TToBuild));
        }

        public object Get(Type typeToBuild)
        {
            return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), typeToBuild, null, null);
        }

        EventBrokerPolicy GetEventBrokerPolicy(Type type)
        {
            EventBrokerPolicy policy = (EventBrokerPolicy)policies.Get<IEventBrokerPolicy>(type, null);

            if (policy == null)
            {
                policy = new EventBrokerPolicy();
                policies.Set<IEventBrokerPolicy>(policy, type, null);
            }

            return policy;
        }

        public object Inject(object @object)
        {
            Guard.ArgumentNotNull(@object, "object");

            return builder.BuildUp(locator, lifetime, policies, strategies.MakeStrategyChain(), @object.GetType(), null, @object);
        }

        void Intercept<T>(Type typeToIntercept,
                          MethodBase method,
                          IInterceptionHandler[] handlers)
            where T : InterceptionPolicy, new()
        {
            T policy = policies.GetLocal<T>(typeToIntercept, null);

            if (policy == null)
            {
                policy = new T();
                policies.Set(policy, typeToIntercept, null);
            }

            policy.Add(method, handlers);
        }

        public void InterceptInterface<T>(MethodInfo interfaceMethod,
                                          params IInterceptionHandler[] handlers)
        {
            Intercept<InterfaceInterceptionPolicy>(typeof(T), interfaceMethod, handlers);
        }

        public void InterceptInterface(Type typeToIntercept,
                                       MethodInfo interfaceMethod,
                                       params IInterceptionHandler[] handlers)
        {
            Intercept<InterfaceInterceptionPolicy>(typeToIntercept, interfaceMethod, handlers);
        }

        public void InterceptRemoting<T>(MethodInfo method,
                                         params IInterceptionHandler[] handlers)
        {
            Intercept<RemotingInterceptionPolicy>(typeof(T), method, handlers);
        }

        public void InterceptRemoting(Type typeToIntercept,
                                      MethodInfo method,
                                      params IInterceptionHandler[] handlers)
        {
            Intercept<RemotingInterceptionPolicy>(typeToIntercept, method, handlers);
        }

        public void InterceptVirtual<T>(MethodInfo method,
                                        params IInterceptionHandler[] handlers)
        {
            Intercept<VirtualInterceptionPolicy>(typeof(T), method, handlers);
        }

        public void InterceptVirtual(Type typeToIntercept,
                                     MethodInfo method,
                                     params IInterceptionHandler[] handlers)
        {
            Intercept<VirtualInterceptionPolicy>(typeToIntercept, method, handlers);
        }

        public void RegisterEventSink<T>(string methodName,
                                         string eventID)
        {
            RegisterEventSink(typeof(T), methodName, eventID);
        }

        public void RegisterEventSink(Type type,
                                      string methodName,
                                      string eventID)
        {
            EventBrokerPolicy policy = GetEventBrokerPolicy(type);
            policy.AddSink(type.GetMethod(methodName), eventID);
        }

        public void RegisterEventSource<T>(string eventName,
                                           string eventID)
        {
            RegisterEventSource(typeof(T), eventName, eventID);
        }

        public void RegisterEventSource(Type type,
                                        string eventName,
                                        string eventID)
        {
            EventBrokerPolicy policy = GetEventBrokerPolicy(type);
            policy.AddSource(type.GetEvent(eventName), eventID);
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

        public void TearDown(object existingObject)
        {
            builder.TearDown(locator, lifetime, policies, strategies.MakeStrategyChain(), existingObject);
        }
    }
}