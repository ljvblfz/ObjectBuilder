using System;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

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

            Assert.True(mock.WasRun);
            Assert.Equal(typeof(SalesFoo), mock.IncomingType);

            mock.WasRun = false;
            mock.IncomingType = null;

            strategy.BuildUp<IFoo>(ctx, null, "marketing");

            Assert.True(mock.WasRun);
            Assert.Equal(typeof(Foo), mock.IncomingType);
        }

        [Test]
        public void CanMapGenericsWithIdenticalGenericParameters()
        {
            MockBuilderContext ctx = new MockBuilderContext();
            TypeMappingStrategy strategy = new TypeMappingStrategy();
            ctx.Policies.Set<ITypeMappingPolicy>(new TypeMappingPolicy(typeof(Foo<>), null), typeof(IFoo<>), null);
            ctx.Strategies.Add(strategy);
            MockStrategy mock = new MockStrategy();
            ctx.Strategies.Add(mock);

            strategy.BuildUp<IFoo<int>>(ctx, null, null);

            Assert.Equal(typeof(Foo<int>), mock.IncomingType);
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

        interface IFoo<T> { }

        class Foo : IFoo {}

        class Foo<T> : IFoo<T> { }

        class SalesFoo : IFoo {}
    }
}