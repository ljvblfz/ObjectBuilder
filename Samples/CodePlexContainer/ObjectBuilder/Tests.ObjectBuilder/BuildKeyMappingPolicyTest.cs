using Xunit;

namespace ObjectBuilder
{
    public class BuildKeyMappingPolicyTest
    {
        [Fact]
        public void PolicyReturnsNewBuildKey()
        {
            BuildKeyMappingPolicy policy = new BuildKeyMappingPolicy(typeof(string));

            Assert.Equal<object>(typeof(string), policy.Map(typeof(object)));
        }
    }
}