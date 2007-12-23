using System;
using Xunit;

namespace ObjectBuilder
{
    public class PropertyReflectionStrategyTest
    {
        [Fact]
        public void DecoratedProperty()
        {
            MockBuilderContext context = new MockBuilderContext();
            PropertyReflectionStrategy strategy = new PropertyReflectionStrategy();

            strategy.BuildUp(context, typeof(Decorated), null);

            IPropertySetterPolicy policy = context.Policies.Get<IPropertySetterPolicy>(typeof(Decorated));
            Assert.NotNull(policy);
            Assert.NotEmpty(policy.Properties);
            foreach (ReflectionPropertySetterInfo property in policy.Properties)
                Assert.Equal(typeof(Decorated).GetProperty("Property"), property.Property);
        }

        [Fact]
        public void NoDecoratedProperties()
        {
            MockBuilderContext context = new MockBuilderContext();
            PropertyReflectionStrategy strategy = new PropertyReflectionStrategy();

            strategy.BuildUp(context, typeof(object), null);

            IPropertySetterPolicy policy = context.Policies.Get<IPropertySetterPolicy>(typeof(object));
            Assert.Null(policy);
        }

        internal class Decorated
        {
            [Dependency]
            public object Property
            {
                set { Console.WriteLine(value); }
            }

            public object UndecoratedProperty
            {
                set { Console.WriteLine(value); }
            }
        }
    }
}