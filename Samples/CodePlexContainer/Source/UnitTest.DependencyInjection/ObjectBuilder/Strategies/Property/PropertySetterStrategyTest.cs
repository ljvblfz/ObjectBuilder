using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class PropertySetterStrategyTest
    {
        [Test]
        public void NoInstance()
        {
            Spy.PropertyValue = null;
            object obj = new object();
            MockBuilderContext context = new MockBuilderContext();
            PropertySetterStrategy strategy = new PropertySetterStrategy();
            PropertySetterPolicy policy = new PropertySetterPolicy();
            policy.Properties.Add(new ReflectionPropertySetterInfo(typeof(Spy).GetProperty("Property"), new ValueParameter<object>(obj)));
            context.Policies.Set<IPropertySetterPolicy>(policy, typeof(Spy));

            strategy.BuildUp(context, typeof(Spy), null);

            Assert.Null(Spy.PropertyValue);
        }

        [Test]
        public void SetsPropertyInPolicy()
        {
            Spy.PropertyValue = null;
            object obj = new object();
            MockBuilderContext context = new MockBuilderContext();
            PropertySetterStrategy strategy = new PropertySetterStrategy();
            PropertySetterPolicy policy = new PropertySetterPolicy();
            policy.Properties.Add(new ReflectionPropertySetterInfo(typeof(Spy).GetProperty("Property"), new ValueParameter<object>(obj)));
            context.Policies.Set<IPropertySetterPolicy>(policy, typeof(Spy));

            strategy.BuildUp(context, typeof(Spy), new Spy());

            Assert.Same(obj, Spy.PropertyValue);
        }

        internal class Spy
        {
            public static object PropertyValue;

            public object Property
            {
                set { PropertyValue = value; }
            }
        }
    }
}