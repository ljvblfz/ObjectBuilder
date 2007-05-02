using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder;

namespace ObjectBuilder.Samples
{
    public class DependencyContainer : IDisposable
    {
        // Fields

        private IBuilder<BuilderStage> builder;
        private LifetimeContainer lifetime;
        private Locator locator;

        // Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContainer"/> class
        /// that supports attribute-based reflection.
        /// </summary>
        public DependencyContainer()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContainer"/> class.
        /// </summary>
        /// <param name="enableReflection">Set to <c>true</c> to enable support for
        /// attribute-based reflection</param>
        public DependencyContainer(bool enableReflection)
            : this(new ContainerXmlConfig(enableReflection))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContainer"/> class.
        /// </summary>
        /// <param name="xml">The XML configuration for the container</param>
        public DependencyContainer(string xml)
            : this(new ContainerXmlConfig(xml))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContainer"/> class.
        /// </summary>
        /// <param name="configurator">The container configurator.</param>
        public DependencyContainer(IBuilderConfigurator<BuilderStage> configurator)
        {
            builder = new BuilderBase<BuilderStage>(configurator);
            locator = new Locator();
            lifetime = new LifetimeContainer();
            locator.Add(typeof(ILifetimeContainer), lifetime);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (lifetime != null)
            {
                List<object> items = new List<object>();
                items.AddRange(lifetime);

                foreach (object item in items)
                    builder.TearDown(locator, item);

                lifetime.Dispose();
                lifetime = null;
                locator = null;
                builder = null;
            }
        }

        // Methods

        /// <summary>
        /// Finds all the items in the container that implement the given type.
        /// </summary>
        /// <returns>An enumeration of the matching items</returns>
        public IEnumerable<T> FindByType<T>()
        {
            foreach (object obj in lifetime)
                if (obj is T)
                    yield return (T)obj;
        }

        /// <summary>
        /// Gets an object of the given type from the container.
        /// </summary>
        /// <returns>The object</returns>
        public TBuild Get<TBuild>()
        {
            return builder.BuildUp<TBuild>(locator, null, null);
        }

        /// <summary>
        /// Registers a singleton instance in the container.
        /// </summary>
        /// <typeparam name="TBuild">The type of the singleton</typeparam>
        /// <param name="item">The item instance to be registered as the singleton</param>
        public void RegisterInstance<TBuild>(TBuild item)
        {
            builder.BuildUp<TBuild>(locator, null, item);
        }

        /// <summary>
        /// Registers the given type as a singleton in the container.
        /// </summary>
        /// <typeparam name="TBuild">The type to be made a singleton</typeparam>
        public void RegisterSingleton<TBuild>()
        {
            builder.Policies.Set<ISingletonPolicy>(new SingletonPolicy(true), typeof(TBuild), null);
        }

        /// <summary>
        /// Registers a type mapping in the container.
        /// </summary>
        /// <typeparam name="TRequested">The type that is requested by the user</typeparam>
        /// <typeparam name="TToBuild">The type to be built instead</typeparam>
        public void RegisterTypeMapping<TRequested, TToBuild>()
        {
            builder.Policies.Set<ITypeMappingPolicy>(new TypeMappingPolicy(typeof(TToBuild), null), typeof(TRequested), null);
        }
    }
}
