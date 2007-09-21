using Xunit;

namespace ObjectBuilder
{
    public class PolicyListTest
    {
        [Test]
        public void CanAddMultiplePoliciesToBagAndRetrieveThem()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy1 = new FakePolicy();
            FakePolicy policy2 = new FakePolicy();

            list.Set<IBuilderPolicy>(policy1, "1");
            list.Set<IBuilderPolicy>(policy2, "2");

            Assert.Same(policy1, list.Get<IBuilderPolicy>("1"));
            Assert.Same(policy2, list.Get<IBuilderPolicy>("2"));
        }

        [Test]
        public void CanAddPolicyToBagAndRetrieveIt()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set<IBuilderPolicy>(policy, typeof(object));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

            Assert.Same(policy, result);
        }

        [Test]
        public void CanClearAllPolicies()
        {
            PolicyList list = new PolicyList();
            list.Set<IBuilderPolicy>(new FakePolicy(), "1");
            list.Set<IBuilderPolicy>(new FakePolicy(), "2");

            list.ClearAll();

            Assert.Equal(0, list.Count);
        }

        [Test]
        public void CanClearDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            list.ClearDefault<IBuilderPolicy>();

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));
            Assert.Null(result);
        }

        [Test]
        public void CanClearPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();

            list.Set<IBuilderPolicy>(policy, typeof(string));
            list.Clear<IBuilderPolicy>(typeof(string));

            Assert.Null(list.Get<IBuilderPolicy>(typeof(string)));
        }

        [Test]
        public void CanGetLocalPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object));

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object), true);

            Assert.Null(result);
        }

        [Test]
        public void CanRegisterGenericPolicyAndRetrieveWithSpecificGenericInstance()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy = new FakePolicy();
            list.Set(policy, typeof(IFoo<>));

            FakePolicy result = list.Get<FakePolicy>(typeof(IFoo<int>));

            Assert.Same(policy, result);
        }

        [Test]
        public void DefaultPolicyUsedWhenSpecificPolicyIsntAvailable()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

            Assert.Same(defaultPolicy, result);
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

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

            Assert.Same(outerPolicy, result);
        }

        [Test]
        public void OuterPolicyOverridesInnerPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object));
            outerList.Set(outerPolicy, typeof(object));

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

            Assert.Same(outerPolicy, result);
        }

        [Test]
        public void SetOverwritesExistingPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy policy1 = new FakePolicy();
            FakePolicy policy2 = new FakePolicy();
            list.Set<IBuilderPolicy>(policy1, typeof(string));
            list.Set<IBuilderPolicy>(policy2, typeof(string));

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(string));

            Assert.Same(policy2, result);
        }

        [Test]
        public void SpecificGenericPolicyComesBeforeGenericPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy genericPolicy = new FakePolicy();
            FakePolicy specificPolicy = new FakePolicy();
            list.Set(genericPolicy, typeof(IFoo<>));
            list.Set(specificPolicy, typeof(IFoo<int>));

            FakePolicy result = list.Get<FakePolicy>(typeof(IFoo<int>));

            Assert.Same(specificPolicy, result);
        }

        [Test]
        public void SpecificInnerPolicyOverridesDefaultOuterPolicy()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy innerPolicy = new FakePolicy();
            FakePolicy outerPolicy = new FakePolicy();
            innerList.Set(innerPolicy, typeof(object));
            outerList.SetDefault(outerPolicy);

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

            Assert.Same(innerPolicy, result);
        }

        [Test]
        public void SpecificPolicyOverridesDefaultPolicy()
        {
            PolicyList list = new PolicyList();
            FakePolicy defaultPolicy = new FakePolicy();
            FakePolicy specificPolicy = new FakePolicy();
            list.Set<IBuilderPolicy>(specificPolicy, typeof(object));
            list.SetDefault<IBuilderPolicy>(defaultPolicy);

            IBuilderPolicy result = list.Get<IBuilderPolicy>(typeof(object));

            Assert.Same(specificPolicy, result);
        }

        [Test]
        public void WillAskInnerPolicyListWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.Set(policy, typeof(object));

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

            Assert.Same(policy, result);
        }

        [Test]
        public void WillUseInnerDefaultPolicyWhenOuterHasNoAnswer()
        {
            PolicyList innerList = new PolicyList();
            PolicyList outerList = new PolicyList(innerList);
            FakePolicy policy = new FakePolicy();
            innerList.SetDefault(policy);

            FakePolicy result = outerList.Get<FakePolicy>(typeof(object));

            Assert.Same(policy, result);
        }

        class FakePolicy : IBuilderPolicy {}

        interface IFoo<T> {}
    }
}