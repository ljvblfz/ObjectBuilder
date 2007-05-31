using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class BuilderContextFixture
    {
        [Test]
        public void TestSettingAndRetrievePolicy()
        {
            PolicyList policies = new PolicyList();
            MockCreationPolicy policy = new MockCreationPolicy();

            policies.Set<IBuilderPolicy>(policy, typeof(object), null);
            BuilderContext context = new BuilderContext(null, null, null, policies);

            IBuilderPolicy outPolicy = context.Policies.Get<IBuilderPolicy>(typeof(object), null);

            Assert.IsNotNull(outPolicy);
            Assert.AreSame(policy, outPolicy);
        }

        [Test]
        public void TestNoPoliciesReturnsNull()
        {
            PolicyList policies = new PolicyList();
            BuilderContext context = new BuilderContext(null, null, null, policies);

            Assert.IsNull(context.Policies.Get<IBuilderPolicy>(typeof(object), null));
        }

        [Test]
        public void PassingNullPoliciesObjectDoesntThrowWhenAskingForPolicy()
        {
            BuilderContext context = new BuilderContext(null, null, null, null);

            Assert.IsNull(context.Policies.Get<IBuilderPolicy>(typeof(object), null));
        }

        [Test]
        public void CanSetPoliciesUsingTheContext()
        {
            BuilderContext context = new BuilderContext(null, null, null, null);
            MockCreationPolicy policy = new MockCreationPolicy();

            context.Policies.Set<IBuilderPolicy>(policy, typeof(object), "foo");

            Assert.AreSame(policy, context.Policies.Get<IBuilderPolicy>(typeof(object), "foo"));
        }

        [Test]
        public void SettingPolicyViaContextDoesNotAffectPoliciesPassedToContextConstructor()
        {
            PolicyList policies = new PolicyList();
            MockCreationPolicy policy1 = new MockCreationPolicy();

            policies.Set<IBuilderPolicy>(policy1, typeof(object), null);
            BuilderContext context = new BuilderContext(null, null, null, policies);

            MockCreationPolicy policy2 = new MockCreationPolicy();
            context.Policies.Set<IBuilderPolicy>(policy2, typeof(string), null);

            Assert.AreEqual(1, policies.Count);
        }

        class MockCreationPolicy : ConstructorPolicy {}
    }
}