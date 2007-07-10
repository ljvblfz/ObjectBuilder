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
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

namespace Farcaster.Tests.Nunit
{
	[TestClass]
	public class StrongTypedStrategiesFixture
	{
		[TestMethod]
		public void MeasurePerformance()
		{
			int iterations = 1000;
			long reflectionBased = 0;
			long stronglyTyped = 0;

			BuilderBase<BuilderStage> builder = new BuilderBase<BuilderStage>();
			builder.Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
			builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
			builder.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
			Locator locator = new Locator();
			IService service = new Service();
			IFoo foo = new Foo();

			locator.Add(new DependencyResolutionLocatorKey(typeof(IService), null), service);
			locator.Add(new DependencyResolutionLocatorKey(typeof(IFoo), null), foo);

			// Build once for warmup
			builder.BuildUp<MockWithCtorDependencies>(locator, null, null);

			Stopwatch watch = Stopwatch.StartNew();
			for (int i = 0; i < iterations; i++)
			{
				builder.BuildUp<MockWithCtorDependencies>(locator, null, null);
			}
			watch.Stop();

			reflectionBased = watch.ElapsedTicks / iterations;

			// Build once for warmup
			PolicyList policies = new PolicyList();
			policies.Set<ICreationPolicy>(new MockWithCtorDependenciesPolicy(), typeof(MockWithCtorDependencies), null);
			builder.BuildUp<MockWithCtorDependencies>(locator, null, null, policies);

			watch = Stopwatch.StartNew();
			for (int i = 0; i < iterations; i++)
			{
				builder.BuildUp<MockWithCtorDependencies>(locator, null, null, policies);
			}
			watch.Stop();

			stronglyTyped = watch.ElapsedTicks / iterations;

			Console.WriteLine("Reflection: {0}\r\nStrongTyped: {1}\r\nTool {2}% the time of the reflection approach.",
				reflectionBased, stronglyTyped, stronglyTyped * 100 / reflectionBased);
		}


		[TestMethod]
		public void CanCreateStronglyTypedConstructor()
		{
			BuilderBase<BuilderStage> builder = new BuilderBase<BuilderStage>();
			builder.Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
			builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
			builder.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
			Locator locator = new Locator();

			IService service = new Service();
			IFoo foo = new Foo();

			locator.Add(new DependencyResolutionLocatorKey(typeof(IService), null), service);
			locator.Add(new DependencyResolutionLocatorKey(typeof(IFoo), null), foo);
			MockWithCtorDependencies mock = builder.BuildUp<MockWithCtorDependencies>(locator, null, null);

			Assert.IsNotNull(mock);
			Assert.AreSame(foo, mock.foo);
			Assert.AreSame(service, mock.service);
		}

		[TestMethod]
		public void CanInjectStronglyTypedConstructor()
		{
			BuilderBase<BuilderStage> builder = new BuilderBase<BuilderStage>();
			builder.Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
			builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
			builder.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
			Locator locator = new Locator();

			IService service = new Service();
			IFoo foo = new Foo();

			locator.Add(new DependencyResolutionLocatorKey(typeof(IService), null), service);
			locator.Add(new DependencyResolutionLocatorKey(typeof(IFoo), null), foo);

			PolicyList policies = new PolicyList();
			policies.Set<ICreationPolicy>(new MockWithCtorDependenciesPolicy(), typeof(MockWithCtorDependencies), null);

			MockWithCtorDependencies mock = builder.BuildUp<MockWithCtorDependencies>(locator, null, null, policies);

			Assert.IsNotNull(mock);
			Assert.AreSame(service, mock.service);
			Assert.AreSame(foo, mock.foo);
		}

		class MockWithCtorDependenciesPolicy : ICreationPolicy
		{
			public ConstructorInfo SelectConstructor(IBuilderContext context, Type type, string id)
			{
				return new StrongTypedConstructor();
			}

			public object[] GetParameters(IBuilderContext context, Type type, string id, ConstructorInfo constructor)
			{
				object[] parameters = new object[2];

				parameters[0] = new DependencyAttribute().CreateParameter(typeof(IService)).GetValue(context);
				parameters[1] = new DependencyAttribute().CreateParameter(typeof(IFoo)).GetValue(context);

				return parameters;
			}

			class StrongTypedConstructor : ConstructorInfo
			{
				public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
				{
					MockWithCtorDependencies mock = (MockWithCtorDependencies)obj;
					mock.service = (IService)parameters[0];
					mock.foo = (IFoo)parameters[1];

					return obj;
				}

				public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
				{
					return new MockWithCtorDependencies(
						(IService)parameters[0],
						(IFoo)parameters[1]);
				}

				public override ParameterInfo[] GetParameters()
				{
					return new ParameterInfo[] {
						new StrongTypedParameter(typeof(IService)), 
						new StrongTypedParameter(typeof(IFoo)) }; 
				}
				
				#region NotSupported Members

				public override MethodAttributes Attributes
				{
					get { throw new NotImplementedException(); }
				}

				public override MethodImplAttributes GetMethodImplementationFlags()
				{
					throw new NotImplementedException();
				}

				public override RuntimeMethodHandle MethodHandle
				{
					get { throw new NotImplementedException(); }
				}

				public override Type DeclaringType
				{
					get { throw new NotImplementedException(); }
				}

				public override object[] GetCustomAttributes(Type attributeType, bool inherit)
				{
					throw new NotImplementedException();
				}

				public override object[] GetCustomAttributes(bool inherit)
				{
					throw new NotImplementedException();
				}

				public override bool IsDefined(Type attributeType, bool inherit)
				{
					throw new NotImplementedException();
				}

				public override string Name
				{
					get { throw new NotImplementedException(); }
				}

				public override Type ReflectedType
				{
					get { throw new NotImplementedException(); }
				}

				#endregion

				class StrongTypedParameter : ParameterInfo
				{
					public StrongTypedParameter(Type parameterType)
					{
						base.ClassImpl = parameterType;
					}
				}
			}
		}

		partial class MockWithCtorDependencies
		{
			public IService service;
			public IFoo foo;

			public MockWithCtorDependencies(
				[Dependency] IService service, 
				[Dependency] IFoo foo)
			{
				this.service = service;
				this.foo = foo;
			}
		}

		interface IService { }
		interface IFoo { }

		class Service : IService { }
		class Foo : IFoo { }
	}
}
