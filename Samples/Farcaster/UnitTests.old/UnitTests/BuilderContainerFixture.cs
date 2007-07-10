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
	public class BuilderContainerFixture
	{
		[TestMethod]
		public void CanConstructContainer()
		{
			BuilderContainer container = new BuilderContainer();

			Assert.IsNotNull(container);
		}

		#region Dispose

		[TestMethod]
		public void CanDisposeContainer()
		{
			BuilderContainer container = new BuilderContainer();
			container.Dispose();
		}

		[TestMethod]
		public void DisposingContainerDisposesManagedSingletonObjects()
		{
			bool disposed = false;
			BuilderContainer container = new BuilderContainer();
			Component c1 = container.BuildUp<Component>("Foo");

			c1.Disposed += delegate { disposed = true; };
			container.Dispose();

			Assert.IsTrue(disposed);
		}

		[TestMethod]
		public void DisposingContainerDoesNotDisposeNonSingletons()
		{
			bool disposed = false;
			BuilderContainer container = new BuilderContainer();
			Component c1 = container.BuildUp<Component>();

			c1.Disposed += delegate { disposed = true; };
			container.Dispose();

			Assert.IsFalse(disposed);
		}

		#endregion

		#region DisposedEvent

		[TestMethod]
		public void DisposedEventListenedByContainerForSingletonComponent()
		{
			BuilderContainer container = new BuilderContainer();
			Component c1 = container.BuildUp<Component>("Foo");

			c1.Dispose();

			Assert.IsFalse(container.Contains("Foo"));
		}

		[TestMethod]
		public void DisposedEventListenedByContainerForSingletonObject()
		{
			BuilderContainer container = new BuilderContainer();
			DisposableFoo foo = container.BuildUp<DisposableFoo>("Foo");

			foo.Dispose();

			Assert.IsFalse(container.Contains("Foo"));
		}

		#endregion

		#region Contains

		[TestMethod]
		public void CanAskContainerContainsById()
		{
			BuilderContainer container = new BuilderContainer();
			Foo f = container.BuildUp<Foo>("Foo");

			Assert.IsTrue(container.Contains("Foo"));
		}

		[TestMethod]
		public void CanAskContainerContainsByType()
		{
			BuilderContainer container = new BuilderContainer();
			Foo f = container.BuildUp<Foo>("Foo");

			Assert.IsTrue(container.Contains(typeof(Foo)));
		}

		[TestMethod]
		public void CanAskContainerContainsByTypeAndId()
		{
			BuilderContainer container = new BuilderContainer();
			Foo f = container.BuildUp<Foo>("Foo");

			Assert.IsTrue(container.Contains(typeof(Foo), "Foo"));
		}

		#endregion

		#region Build

		[TestMethod]
		public void CanBuildObject()
		{
			BuilderContainer container = new BuilderContainer();

			Foo f = container.BuildUp<Foo>();

			Assert.IsNotNull(f);
		}

		[TestMethod]
		public void BuildUpWithoutIdIsNotContainerManaged()
		{
			BuilderContainer container = new BuilderContainer();

			Foo f = container.BuildUp<Foo>();

			Assert.IsNotNull(f);
			Assert.IsFalse(((ILifetimeContainer)container).Contains(f));
		}

		[TestMethod]
		public void BuildUpWithIdIsContainerManagedAndSingleton()
		{
			BuilderContainer container = new BuilderContainer();

			Foo f = container.BuildUp<Foo>("foo");
			Foo f2 = container.BuildUp<Foo>("foo");

			Assert.AreSame(f, f2);
		}

		#endregion

		#region Apply

		[TestMethod]
		public void ApplyBuilderWithoutIdIsNotContainerManaged()
		{
			BuilderContainer container = new BuilderContainer();

			Foo f = new Foo();

			container.ApplyBuilder<Foo>(f);

			Assert.IsFalse(((ILifetimeContainer)container).Contains(f));
		}

		[TestMethod]
		public void ApplyBuilderWithIdIsContainerManagedAndSingleton()
		{
			BuilderContainer container = new BuilderContainer();

			Foo f = new Foo();

			container.ApplyBuilder<Foo>(f, "foo");
			Foo f2 = container.BuildUp<Foo>("foo");

			Assert.IsTrue(((ILifetimeContainer)container).Contains(f));
			Assert.AreSame(f, f2);
		}

		#endregion

		#region Register

		[TestMethod]
		public void CanRegisterImplementationType()
		{
			BuilderContainer container = new BuilderContainer();

			container.Register<IFoo, Foo>();

			IFoo foo = container.BuildUp<IFoo>();

			Assert.IsTrue(foo is Foo);
		}

		[TestMethod]
		public void CanRegisterImplementationTypeForId()
		{
			BuilderContainer container = new BuilderContainer();

			container.Register<IFoo, Foo>();
			container.Register<IFoo, Foo2>("Foo2");

			IFoo foo = container.BuildUp<IFoo>("Foo2");

			Assert.IsTrue(foo is Foo2);
		}

		// Does not work
		//[TestMethod]
		//public void CanRegisterImplementationTypeForAnyId()
		//{
		//    BuilderContainer container = new BuilderContainer();

		//    container.Register<IFoo, Foo>();

		//    IFoo foo = container.BuildUp<IFoo>("foo");

		//    Assert.IsTrue(foo is Foo);
		//}

		#endregion

		#region Helper classes

		interface IFoo { }

		class Foo : IFoo { }
		class Foo2 : IFoo { }

		class DisposableFoo : IDisposable
		{
			public event EventHandler Disposed;

			public void Dispose()
			{
				if (Disposed != null)
					Disposed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
