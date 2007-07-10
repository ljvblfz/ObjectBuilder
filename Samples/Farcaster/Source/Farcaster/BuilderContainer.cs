using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;

namespace Farcaster
{
	/// <summary>
	/// A lifetime and locator container that uses an object builder for object creation.
	/// </summary>
	public class BuilderContainer : IDisposable, ILifetimeContainer
	{
		ILifetimeContainer lifetime;
		IReadWriteLocator locator;
		BuilderBase<BuilderStage> builder;
		IBuilderConfigurator<BuilderStage> configurator;

		/// <summary>
		/// Initializes a new instance of the container using a default builder configuration 
		/// that has a default <see cref="CreationStrategy"/> strategy.
		/// </summary>
		public BuilderContainer() : this(new DefaultConfigurator())
		{
		}

		/// <summary>
		/// Initializes a new instance of the container using the specified configurator.
		/// </summary>
		/// <param name="configurator">The configurator that provides default 
		/// strategies for the container.</param>
		public BuilderContainer(IBuilderConfigurator<BuilderStage> configurator)
		{
			Guard.ArgumentNotNull(configurator, "configurator");

			lifetime = new LifetimeContainer();
			locator = new Locator();
			locator.Add(typeof(ILifetimeContainer), lifetime);
			builder = new BuilderBase<BuilderStage>(configurator);
			this.configurator = configurator;
		}

		private BuilderContainer(BuilderContainer parent)
		{
			builder = new BuilderBase<BuilderStage>(parent.configurator);
			lifetime = new LifetimeContainer();
			locator = new Locator(parent.locator);
			locator.Add(typeof(ILifetimeContainer), lifetime);
		}

		/// <summary>
		/// Builds an anonymous, non-singleton, non-lifetime managed object of type 
		/// <typeparamref name="TTypeToBuild"/>.
		/// </summary>
		/// <typeparam name="TTypeToBuild">The type of the object to build.</typeparam>
		/// <returns></returns>
		public TTypeToBuild BuildUp<TTypeToBuild>()
		{
			return builder.BuildUp<TTypeToBuild>(locator, null, null);
		}

		/// <summary>
		/// Builds an object with the given <c>id</c>, and makes it a singleton 
		/// in the container, which also manages its lifetime.
		/// </summary>
		/// <typeparam name="TTypeToBuild">The type of the object to build.</typeparam>
		/// <param name="objectId">The <c>id</c> of the object to build.</param>
		/// <exception cref="ArgumentNullException"><paramref name="objectId"/> is null.</exception>
		public TTypeToBuild BuildUp<TTypeToBuild>(string objectId)
		{
			Guard.ArgumentNotNull(objectId, "objectId");
			PolicyList policies = new PolicyList();
			policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));

			return builder.BuildUp<TTypeToBuild>(locator, objectId, null, policies);
		}

		/// <summary>
		/// Applies the builder strategies to an existing anonymous, non-singleton, 
		/// non-lifetime managed object.
		/// </summary>
		/// <typeparam name="TTypeToBuild">The type being built, which may not be the 
		/// concrete type of the object instance (i.e. an interface).</typeparam>
		/// <param name="existing">The existing object to apply strategies to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="existing"/> is null.</exception>
		public void ApplyBuilder<TTypeToBuild>(TTypeToBuild existing)
		{
			Guard.ArgumentNotNull(existing, "existing");
			builder.BuildUp<TTypeToBuild>(locator, null, existing);
		}

		/// <summary>
		/// Applies the builder strategies to an existing object with the given <c>id</c>, 
		/// and makes it a singleton in the container, which also manages its lifetime.
		/// </summary>
		/// <typeparam name="TTypeToBuild">The type being built, which may not be the 
		/// concrete type of the object instance (i.e. an interface).</typeparam>
		/// <param name="existing">The existing object to apply strategies to.</param>
		/// <param name="objectId">The <c>id</c> of the object the strategies will be applied to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="existing"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="objectId"/> is null.</exception>
		public void ApplyBuilder<TTypeToBuild>(TTypeToBuild existing, string objectId)
		{
			Guard.ArgumentNotNull(existing, "existing");
			Guard.ArgumentNotNull(objectId, "objectId");

			PolicyList policies = new PolicyList();
			policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));
			
			builder.BuildUp<TTypeToBuild>(locator, objectId, existing, policies);
		}

		/// <summary>
		/// Determines if at least one object with the given <paramref name="objectId"/> exists
		/// in the container.
		/// </summary>
		/// <param name="objectId">The identifier to search for.</param>
		/// <returns><see langword="true"/> if the key exists in the container; <see langword="false"/> otherwise.</returns>
		/// <remarks>
		/// Container-managed objects are always singletons by id and type. Therefore, 
		/// <paramref name="objectId"/> is not guaranteed to be unique in the container, 
		/// only within that pair id/type.
		/// </remarks>
		public bool Contains(string objectId)
		{
			IReadableLocator results = locator.FindBy(delegate(KeyValuePair<object, object> entry)
			{
				string stringKey = entry.Key as string;
				if (stringKey != null && stringKey == objectId)
				{
					return true;
				}

				DependencyResolutionLocatorKey key = entry.Key as DependencyResolutionLocatorKey;
				if (key != null && key.ID == objectId)
				{
					return true;
				}

				return false;
			});

			return results.Count > 0;
		}

		/// <summary>
		/// Determines if at least one object of the given <paramref name="type"/> exists 
		/// in the container.
		/// </summary>
		/// <param name="type">The type of the object to search for.</param>
		/// <returns><see langword="true"/> if the key exists in the container; <see langword="false"/> otherwise.</returns>
		public bool Contains(Type type)
		{
			IReadableLocator results = locator.FindBy(delegate(KeyValuePair<object, object> entry)
			{
				Type typeKey = entry.Key as Type;
				if (typeKey != null && typeKey == type)
				{
					return true;
				}

				DependencyResolutionLocatorKey key = entry.Key as DependencyResolutionLocatorKey;
				if (key != null && key.Type == type)
				{
					return true;
				}

				return false;
			});

			return results.Count > 0;
		}

		/// <summary>
		/// Determines if an object of the given <paramref name="type"/> and with 
		/// the specified <paramref name="objectId"/> exists
		/// in the container.
		/// </summary>
		/// <param name="objectId">The identifier to search for.</param>
		/// <param name="type">The type of the object to search for.</param>
		/// <returns><see langword="true"/> if the key exists in the container; <see langword="false"/> otherwise.</returns>
		public bool Contains(Type type, string objectId)
		{
			return locator.Contains(new DependencyResolutionLocatorKey(type, objectId));
		}

		/// <summary>
		/// Registers the type <typeparamref name="TImplementation"/> with the 
		/// key <typeparamref name="TRegisterAs"/>.
		/// </summary>
		public void Register<TRegisterAs, TImplementation>()
			where TImplementation : TRegisterAs
		{
			builder.Policies.Set<ITypeMappingPolicy>(
				new TypeMappingPolicy(typeof(TImplementation), null),
				typeof(TRegisterAs), null);
		}

		/// <summary>
		/// Registers the type <typeparamref name="TImplementation"/> with the 
		/// key <typeparamref name="TRegisterAs"/> for a specific object identifier.
		/// </summary>
		public void Register<TRegisterAs, TImplementation>(string objectId)
			where TImplementation : TRegisterAs
		{
			builder.Policies.Set<ITypeMappingPolicy>(
				new TypeMappingPolicy(typeof(TImplementation), objectId),
				typeof(TRegisterAs), objectId);
		}

		#region IDisposable Members

		/// <summary>
		/// Disposes the container and all singleton (named) objects contained in it.
		/// </summary>
		/// <summary>
		/// Disposes the container, and any objects contained in the container.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Disposes the objects in the container.
		/// </summary>
		/// <param name="disposing">True if called from Dispose(); false if called from
		/// a finalizer.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				lifetime.Dispose();
			}
		}

		#endregion

		#region ILifetimeContainer Members

		/// <summary>
		/// See <see cref="ILifetimeContainer.Count"/>.
		/// </summary>
		int ILifetimeContainer.Count
		{
			get { return lifetime.Count; }
		}

		/// <summary>
		/// See <see cref="ILifetimeContainer.Add"/>.
		/// </summary>
		void ILifetimeContainer.Add(object item)
		{
			lifetime.Add(item);
		}

		/// <summary>
		/// See <see cref="ILifetimeContainer.Contains"/>.
		/// </summary>
		bool ILifetimeContainer.Contains(object item)
		{
			return lifetime.Contains(item);
		}

		/// <summary>
		/// See <see cref="ILifetimeContainer.Remove"/>.
		/// </summary>
		void ILifetimeContainer.Remove(object item)
		{
			lifetime.Remove(item);
		}

		#endregion

		#region IEnumerable<object> Members

		/// <summary>
		/// Enumerates the items in the container.
		/// </summary>
		IEnumerator<object> IEnumerable<object>.GetEnumerator()
		{
			return lifetime.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<object>)this).GetEnumerator();
		}

		#endregion
	}
}
