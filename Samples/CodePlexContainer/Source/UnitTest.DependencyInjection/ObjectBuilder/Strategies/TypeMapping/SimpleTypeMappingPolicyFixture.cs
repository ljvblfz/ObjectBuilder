using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class SimpleTypeMappingPolicyFixture
    {
        [Test]
        public void PolicyReturnsGivenType()
        {
            TypeMappingPolicy policy = new TypeMappingPolicy(typeof(Foo), null);

            Assert.AreEqual(new DependencyResolutionLocatorKey(typeof(Foo), null), policy.Map(new DependencyResolutionLocatorKey()));
        }

        class Foo {}
    }
}