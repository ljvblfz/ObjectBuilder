#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.ComponentModel;

namespace Farcaster.Tests.Nunit
{
	[TestClass]
	public class BuilderContainerComponentSupport
	{
		[TestMethod]
		public void BuildSetsSite()
		{
			BuilderContainer container = new BuilderContainer();

			Component component = container.BuildUp<Component>("Foo");

			Assert.IsNotNull(component.Site);
		}

		[TestMethod]
		public void SiteNameMatchesBuiltId()
		{
			BuilderContainer container = new BuilderContainer();

			Component component = container.BuildUp<Component>("Foo");

			Assert.AreEqual("Foo", component.Site.Name);
		}
		
		[TestMethod]
		public void DisposeRemovesSite()
		{
			BuilderContainer container = new BuilderContainer();
			Component component = container.BuildUp<Component>("Foo");

			component.Dispose();

			Assert.IsNull(component.Site);
		}

		[TestMethod]
		public void DisposeRemovesFromContainer()
		{
			BuilderContainer container = new BuilderContainer();
			Component component = container.BuildUp<Component>("Foo");

			component.Dispose();

			Assert.IsFalse(container.Contains("Foo"));
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void CannotSetComponentNameThroughSite()
		{
			BuilderContainer container = new BuilderContainer();
			Component component = container.BuildUp<Component>("Foo");

			component.Site.Name = "Bar";
		}

		[TestMethod]
		public void GetServiceLocatesInContainerByType()
		{
			BuilderContainer container = new BuilderContainer();
			FooComponent component = container.BuildUp<FooComponent>("Foo");

			object service = component.GetService<ILifetimeContainer>();

			Assert.IsNotNull(service);
		}

		#region Helper classes

		class FooComponent : Component
		{
			public T GetService<T>()
			{
				return (T)GetService(typeof(T));
			}
		}

		#endregion
	}
}
