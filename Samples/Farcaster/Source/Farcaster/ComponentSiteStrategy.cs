using System;
using System.ComponentModel;
using Microsoft.Practices.ObjectBuilder;
using System.Reflection;

namespace Farcaster
{
	/// <summary>
	/// Strategy that provides an <see cref="ISite"/> implementation to 
	/// <see cref="IComponent.Site"/> property, so that regular components 
	/// can locate services on the container without requiring any dependencies 
	/// on Object Builder.
	/// </summary>
	/// <remarks>
	/// This strategy is applied to all components regardless of their singleton behavior.
	/// </remarks>
	public class ComponentSiteStrategy : BuilderStrategy
	{
		/// <summary>
		/// Attaches to a <c>Disposed</c> event in the object being built.
		/// </summary>
		public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
		{
			object result = base.BuildUp(context, typeToBuild, existing, idToBuild);

			IComponent component = result as IComponent;
			if (component != null)
			{
				LocatorContainer container = new LocatorContainer(context.Locator);
				// We don't need to keep a reference to the container alive as the 
				// component keeps it already, via its Site.Container property. 
				// This will ensure the container is not disposed and access to 
				// services can be performed. The moment the component is 
				// disposed, the default implementation in Component is to 
				// remove the component from the container and set the Site to 
				// null, therefore releasing any remaining references to the container, 
				// which can now be GC'ed.
				container.Add(component, idToBuild);
			}

			return result;
		}

		class LocatorContainer : Container
		{
			IReadableLocator locator;

			public LocatorContainer(IReadableLocator locator)
			{
				this.locator = locator;
			}

			protected override object GetService(Type service)
			{
				return locator.Get(service);
			}

			protected override void ValidateName(IComponent component, string name)
			{
				if (component.Site != null &&
					component.Site.Name != name)
				{
					throw new NotSupportedException(Properties.Resources.CannotChangeComponentSiteName);
				}

				base.ValidateName(component, name);
			}
		}
	}
}
