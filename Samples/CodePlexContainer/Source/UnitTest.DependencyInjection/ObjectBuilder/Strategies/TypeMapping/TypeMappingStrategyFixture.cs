using System;
using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class TypeMappingStrategyFixture
    {
        [Test]
        public void CanMapInterfacesToConcreteTypes()
        {
            MockBuilderContext ctx = new MockBuilderContext();
            TypeMappingStrategy strategy = new TypeMappingStrategy();
            ctx.Policies.Set<ITypeMappingPolicy>(new TypeMappingPolicy(typeof(SalesFoo), null), typeof(IFoo), "sales");
            ctx.Policies.Set<ITypeMappingPolicy>(new TypeMappingPolicy(typeof(Foo), null), typeof(IFoo), "marketing");
            ctx.Strategies.Add(strategy);

            MockStrategy mock = new MockStrategy();
            ctx.Strategies.Add(mock);

            strategy.BuildUp<IFoo>(ctx, null, "sales");

            Assert.IsTrue(mock.WasRun);
            Assert.AreEqual(typeof(SalesFoo), mock.IncomingType);

            mock.WasRun = false;
            mock.IncomingType = null;

            strategy.BuildUp<IFoo>(ctx, null, "marketing");

            Assert.IsTrue(mock.WasRun);
            Assert.AreEqual(typeof(Foo), mock.IncomingType);
        }

        [Test]
        [ExpectedException(typeof(IncompatibleTypesException))]
        public void IncompatibleTypes()
        {
            MockBuilderContext ctx = new MockBuilderContext();
            TypeMappingStrategy strategy = new TypeMappingStrategy();
            ctx.Policies.Set<ITypeMappingPolicy>(new TypeMappingPolicy(typeof(object), null), typeof(IFoo), "sales");
            ctx.Strategies.Add(strategy);

            strategy.BuildUp<IFoo>(ctx, null, "sales");
        }

        class MockStrategy : BuilderStrategy
        {
            public Type IncomingType = null;
            public bool WasRun = false;

            public override object BuildUp(IBuilderContext context,
                                           Type t,
                                           object existing,
                                           string id)
            {
                WasRun = true;
                IncomingType = t;
                return null;
            }
        }

        interface IFoo {}

        class Foo : IFoo {}

        class SalesFoo : IFoo {}
    }
}