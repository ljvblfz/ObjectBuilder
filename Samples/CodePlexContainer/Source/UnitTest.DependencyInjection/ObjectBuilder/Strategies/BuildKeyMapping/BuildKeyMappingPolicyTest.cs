using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class BuildKeyMappingPolicyTest
    {
        [Test]
        public void PolicyReturnsNewBuildKey()
        {
            BuildKeyMappingPolicy policy = new BuildKeyMappingPolicy(typeof(string));

            Assert.Equal<object>(typeof(string), policy.Map(typeof(object)));
        }
    }
}