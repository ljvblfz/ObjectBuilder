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

namespace Farcaster.Tests.Nunit
{
	[TestClass]
	public class PropertySetterInjectionFixture
	{
		const int iterations = 100;

		[TestMethod]
		public void LCGInjectionIsFaster()
		{
			Locator locator = new Locator();
			BuilderBase<BuilderStage> builder = new BuilderBase<BuilderStage>();
			builder.Strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
			builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
			builder.Strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);

			PropertySetterPolicy policy = new PropertySetterPolicy();
			policy.Properties.Add("B", new PropertySetterInfo("B", new CreationParameter(typeof(Bar))));
			builder.Policies.Set<IPropertySetterPolicy>(policy, typeof(Foo), null);

			long ticks = 0;

			for (int i = 0; i < iterations; i++)
			{
				Stopwatch watch = new Stopwatch();
				watch.Start();
				Foo a = builder.BuildUp<Foo>(locator, null, null);
				Assert.IsNotNull(a.B);
				watch.Stop();

				if (i % 100 == 0)
				{
					GC.Collect();
				}

				ticks += watch.ElapsedTicks;
			}

			long avg = ticks / iterations;

			locator = new Locator();
			builder = new BuilderBase<BuilderStage>();
			builder.Strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
			builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
			builder.Strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);

			policy = new PropertySetterPolicy();
			policy.Properties.Add("B", new FastPropertySetterInfo("B", new CreationParameter(typeof(Bar))));
			builder.Policies.Set<IPropertySetterPolicy>(policy, typeof(Foo), null);

			ticks = 0;

			for (int i = 0; i < iterations; i++)
			{
				Stopwatch watch = new Stopwatch();
				watch.Start();
				Foo a = builder.BuildUp<Foo>(locator, null, null);
				Assert.IsNotNull(a.B);
				watch.Stop();

				if (i % 100 == 0)
				{
					GC.Collect();
				}

				ticks += watch.ElapsedTicks;
			}

			long avg2 = ticks / iterations;

			Console.WriteLine("{0} vs {1}", avg, avg2);

			Assert.IsTrue(avg2 < avg);
		}

		[TestMethod]
		public void CanSetPropertyValueWithFastInfo()
		{
			FastPropertyInfo propInfo = new FastPropertyInfo(typeof(Foo).GetProperty("B"));

			Foo a = new Foo();
			Bar b = new Bar();

			propInfo.SetValue(a, b, null);

			Assert.AreSame(b, a.B);
		}

		[TestMethod]
		public void CanGetPropertyValueFromFastInfo()
		{
			FastPropertyInfo propInfo = new FastPropertyInfo(typeof(Foo).GetProperty("B"));

			Foo a = new Foo();
			Bar b = new Bar();
			a.B = b;

			object b2 = propInfo.GetValue(a, null);

			Assert.AreSame(b, b2);
		}

		#region Helper classes

		public class Foo
		{
			private Bar b;

			public Bar B
			{
				get { return b; }
				set { b = value; }
			}
		}

		public class Bar
		{
		}

		#endregion
	}
}
