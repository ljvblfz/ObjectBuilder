using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class LookupParameterFixture
    {
        [Test]
        public void ConstructorPolicyCanUseLookupToFindAnObject()
        {
            MockBuilderContext ctx = new MockBuilderContext();
            ctx.Locator.Add("foo", 11);

            LookupParameter param = new LookupParameter("foo");

            Assert.AreEqual(11, param.GetValue(ctx));
            Assert.AreSame(typeof(int), param.GetParameterType(ctx));
        }
    }
}