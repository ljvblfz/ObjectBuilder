using System;
using System.ComponentModel;
using Microsoft.Practices.ObjectBuilder;
using System.Reflection;

namespace Farcaster
{
	/// <summary>
	/// Strategy that attaches to the <see cref="IComponent.Disposed"/> event or a 
	/// similarly named event if is compatible with the <see cref="EventHandler"/> signature. 
	/// This event is subscribed only for singleton objects in the container, as 
	/// non-singletons are not lifetime-managed.
	/// </summary>
	public class SingletonDisposeEventStrategy : BuilderStrategy
	{
		/// <summary>
		/// Attaches to a <c>Disposed</c> event in the object being built.
		/// </summary>
		public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
		{
			object result = base.BuildUp(context, typeToBuild, existing, idToBuild);

			ISingletonPolicy policy = context.Policies.Get<ISingletonPolicy>(typeToBuild, idToBuild);
			
			if (result != null && policy != null && policy.IsSingleton)
			{
				IComponent component = result as IComponent;
				if (component != null)
				{
					component.Disposed += new DisposedHandlerClosure(context.Locator,
						new DependencyResolutionLocatorKey(typeToBuild, idToBuild)).OnDisposed;
				}
				else
				{
					EventInfo disposedEvent = result.GetType().GetEvent("Disposed");
					if (disposedEvent != null)
					{
						try
						{
							disposedEvent.AddEventHandler(result, new EventHandler(
								new DisposedHandlerClosure(context.Locator, 
									new DependencyResolutionLocatorKey(typeToBuild, idToBuild)).OnDisposed));
						}
						catch
						{
							// Disposed event is not an EventHandler
						}
					}
				}
			}

			return result;
		}

		class DisposedHandlerClosure
		{
			IReadWriteLocator locator;
			DependencyResolutionLocatorKey key;

			public DisposedHandlerClosure(IReadWriteLocator locator, DependencyResolutionLocatorKey key)
			{
				this.locator = locator;
				this.key = key;
			}

			public void OnDisposed(object sender, EventArgs e)
			{
				locator.Remove(key);
				ILifetimeContainer lifetime = locator.Get<ILifetimeContainer>();
				if (lifetime != null)
				{
					lifetime.Remove(sender);
				}
			}
		}
	}
}
