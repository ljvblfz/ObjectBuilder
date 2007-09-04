using System;
using ObjectBuilder;

namespace Ottawa
{
    public class OttawaContainer : IOttawaContainer
    {
        readonly Builder builder;
        readonly LifetimeContainer lifetime;
        readonly Locator locator;
        readonly PolicyList policies;
        readonly StrategyChain strategies;

        public OttawaContainer()
        {
            builder = new Builder();
            locator = new Locator();
            lifetime = new LifetimeContainer();
            policies = new PolicyList();
            strategies = new StrategyChain();

            strategies.Add(new SingletonStrategy());
            strategies.Add(new BuildKeyMappingStrategy());
            strategies.Add(new CreationStrategy());

            policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));
        }

        public object this[Type service]
        {
            get { return Resolve(service); }
        }

        public object this[string key]
        {
            get { throw new NotImplementedException(); }
        }

        public void AddComponent<TClass>(string key)
        {
            AddComponentWithLifestyle(key, typeof(TClass), typeof(TClass), LifestyleType.Singleton);
        }

        public void AddComponent(string key,
                                 Type classType)
        {
            AddComponentWithLifestyle(key, classType, classType, LifestyleType.Singleton);
        }

        public void AddComponent<TService, TClass>(string key)
        {
            AddComponentWithLifestyle(key, typeof(TService), typeof(TClass), LifestyleType.Singleton);
        }

        public void AddComponent(string key,
                                 Type serviceType,
                                 Type classType)
        {
            AddComponentWithLifestyle(key, serviceType, classType, LifestyleType.Singleton);
        }

        public void AddComponentWithLifestyle<TClass>(string key,
                                                      LifestyleType lifestyle)
        {
            AddComponentWithLifestyle(key, typeof(TClass), typeof(TClass), lifestyle);
        }

        public void AddComponentWithLifestyle(string key,
                                              Type classType,
                                              LifestyleType lifestyle)
        {
            AddComponentWithLifestyle(key, classType, classType, lifestyle);
        }

        public void AddComponentWithLifestyle<TService, TClass>(string key,
                                                                LifestyleType lifestyle)
        {
            AddComponentWithLifestyle(key, typeof(TService), typeof(TClass), lifestyle);
        }

        public void AddComponentWithLifestyle(string key,
                                              Type serviceType,
                                              Type classType,
                                              LifestyleType lifestyle)
        {
            Guard.ArgumentNotNull(key, "key");
            // Guard for service type
            Guard.ArgumentNotNull(classType, "classType");

            if (policies.GetNoDefault<IBuildKeyMappingPolicy>(key, true) != null)
                throw new ComponentRegistrationException(key);

            if (serviceType != classType && policies.GetNoDefault<IBuildKeyMappingPolicy>(serviceType, true) == null)
                policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(classType), serviceType);

            if (lifestyle == LifestyleType.Transient)
            {
                policies.Set<ISingletonPolicy>(new SingletonPolicy(false), key);
                policies.Set<ISingletonPolicy>(new SingletonPolicy(false), serviceType);
                policies.Set<ISingletonPolicy>(new SingletonPolicy(false), classType);
            }

            policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(classType), key);
            policies.Set<ICreationPolicy>(new OttawaCreationPolicy(), classType);
        }

        public void Dispose()
        {
            lifetime.Dispose();
        }

        public void Release(object instance) {}

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public T Resolve<T>(string key)
        {
            return (T)Resolve(key);
        }

        public object Resolve(string key)
        {
            try
            {
                return builder.BuildUp(locator, lifetime, policies, strategies, key, null);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Could not find creation policy"))
                    throw new ComponentNotFoundException(key);

                throw;
            }
        }

        public object Resolve(Type service)
        {
            try
            {
                return builder.BuildUp(locator, lifetime, policies, strategies, service, null);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Could not find creation policy"))
                    throw new ComponentNotFoundException(service);

                throw;
            }
            catch (MissingMethodException)
            {
                throw new ComponentActivatorException(service);
            }
            //catch (MissingMethodException)
            //{
            //    if (service.IsInterface)
            //        throw new ArgumentException("Cannot create interface");

            //    if (service.IsAbstract)
            //        throw new ArgumentException("Cannot create abstract class");

            //    throw;
            //}
        }

        public object Resolve(string key,
                              Type service)
        {
            throw new NotImplementedException();
        }
    }
}