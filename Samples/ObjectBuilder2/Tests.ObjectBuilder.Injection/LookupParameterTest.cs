using Xunit;

namespace ObjectBuilder
{
    public class LookupParameterTest
    {
        [Fact]
        public void ConstructorPolicyCanUseLookupToFindAnObject()
        {
            MockBuilderContext ctx = new MockBuilderContext();
            ctx.Locator.Add("foo", 11);

            LookupParameter param = new LookupParameter("foo");

            Assert.Equal<object>(11, param.GetValue(ctx));
            Assert.Same(typeof(int), param.GetParameterType(ctx));
        }
    }
}