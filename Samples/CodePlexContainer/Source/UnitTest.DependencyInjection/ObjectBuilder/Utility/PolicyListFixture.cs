using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class PolicyListFixture
    {
        [Test]
        public void CanAddPolicyToBagAndRetrieveIt()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set<IBuilderPolicy>(policy, typeof(object), null);

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object), null);

            Assert.Same(policy, result);
        }

        [Test]
        public void CanAddMultiplePoliciesToBagAndRetrieveThem()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy1 = new FakePolicy();
            FakePolicy policy2 = new FakePolicy();

            list.Set<IBuilderPolicy>(policy1, typeof(object), "1");
            list.Set<IBuilderPolicy>(policy2, typeof(string), "2");

            Assert.Same(policy1, list.Get<IBuilderPolicy>(typeof(object), "1"));
            Assert.Same(policy2, list.Get<IBuilderPolicy>(typeof(string), "2"));
        }

        [Test]
        public void SetOverwritesExistingPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy1 = new FakePolicy();
            FakePolicy policy2 = new FakePolicy();
            list.Set<IBuilderPolicy>(policy1, typeof(string), "1");
            list.Set<IBuilderPolicy>(policy2, typeof(string), "1");

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(string), "1");

            Assert.Same(policy2, result);
        }

        [Test]
        public void CanClearPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();

            list.Set<IBuilderPolicy>(policy, typeof(string), "1");
            list.Clear<IBuilderPolicy>(typeof(string), "1");

            Assert.Null(list.Get<IBuilderPolicy>(typeof(string), "1"));
        }

        [Test]
        public void CanClearAllPolicies()
        {
            PolicyList list = new PolicyList();
            list.Set<IBuilderPolicy>(new FakePolicy(), typeof(object), null);
            list.Set<IBuilderPolicy>(new FakePolicy(), typeof(object), "1");

            list.ClearAll();

            Assert.Equal(0, list.Count);
        }

        [Test]
        public void DefaultPolicyUsedWhenSpecificPolicyIsntAvailable()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object), null);

            Assert.Same(defaultPolicy, result);
        }

        [Test]
        public void SpecificPolicyOverridesDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            FakePolicy specificPolicy = new FakePolicy();
            list.Set<IBuilderPolicy>(specificPolicy, typeof(object), null);
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object), null);

            Assert.Same(specificPolicy, result);
        }

        [Test]
        public void CanClearDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.SetDefault<IBuilderPolicy>(defaultPolicy);
            list.ClearDefault<IBuilderPolicy>();

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object), null);

            Assert.Null(result);
        }

        [Test]
        public void WillAskInnerPolicyListWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.Set(policy, typeof(object), null);

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), null);

            Assert.Same(policy, result);
        }

        [Test]
        public void WillUseInnerDefaultPolicyWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.SetDefault(policy);

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), null);

            Assert.Same(policy, result);
        }

        [Test]
        public void OuterPolicyOverridesInnerPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object), null);
            outerList.Set(outerPolicy, typeof(object), null);

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), null);

            Assert.Same(outerPolicy, result);
        }

        [Test]
        public void SpecificInnerPolicyOverridesDefaultOuterPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object), null);
            outerList.SetDefault(outerPolicy);

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), null);

            Assert.Same(innerPolicy, result);
        }

        [Test]
        public void OuterPolicyDefaultOverridesInnerPolicyDefault()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.SetDefault(innerPolicy);
            outerList.SetDefault(outerPolicy);

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), null);

            Assert.Same(outerPolicy, result);
        }

        [Test]
        public void NullNameIsSameAsEmptyStringName()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set(policy, typeof(object), null);

            FakePolicy result1 = list.Get<FakePolicy>(typeof(object), null);
            FakePolicy result2 = list.Get<FakePolicy>(typeof(object), "");

            Assert.Same(result1, result2);
        }

        [Test]
        public void EmptyStringNameIsSameAsNullName()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set(policy, typeof(object), "");

            FakePolicy result1 = list.Get<FakePolicy>(typeof(object), null);
            FakePolicy result2 = list.Get<FakePolicy>(typeof(object), "");

            Assert.Same(result1, result2);
        }

        [Test]
        public void CanSetDefaultPolicyForTypes()
        {
            PolicyList list = new PolicyList();
            FakePolicy policyAllTypesDefault = new FakePolicy();
            FakePolicy policyTypeDefault = new FakePolicy();
            FakePolicy policyNamedType = new FakePolicy();
            list.SetDefault(policyAllTypesDefault);
            list.SetDefaultForType(policyTypeDefault, typeof(object));
            list.Set(policyNamedType, typeof(object), "foo");

            FakePolicy resultAll = list.Get<FakePolicy>(typeof(int), null);
            FakePolicy resultTyped = list.Get<FakePolicy>(typeof(object), "bar");
            FakePolicy resultNamed = list.Get<FakePolicy>(typeof(object), "foo");

            Assert.Same(policyAllTypesDefault, resultAll);
            Assert.Same(policyTypeDefault, resultTyped);
            Assert.Same(policyNamedType, resultNamed);
        }

        [Test]
        public void CanGetLocalPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object), null);

            FakePolicy result = outerList.GetLocal<FakePolicy>(typeof(object), null);

            Assert.Null(result);
        }

        class FakePolicy : IBuilderPolicy {}
    }
}