using System.Reflection;
using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class ConstructorPolicyFixture
    {
        [Test]
        public void GetConstructorReturnsTheCorrectOneWhenParamsPassedThruAddParameter()
        {
            ConstructorPolicy policy = new ConstructorPolicy();

            policy.AddParameter(new ValueParameter<int>(5));
            ConstructorInfo actual = policy.SelectConstructor(new MockBuilderContext(), typeof(MockObject), null);
            ConstructorInfo expected = typeof(MockObject).GetConstructors()[1];

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void GetConstructorReturnsTheCorrectOneWhenParamsPassedThruCtor()
        {
            ConstructorPolicy policy = new ConstructorPolicy(new ValueParameter<int>(5));

            ConstructorInfo actual = policy.SelectConstructor(new MockBuilderContext(), typeof(MockObject), null);
            ConstructorInfo expected = typeof(MockObject).GetConstructors()[1];

            Assert.AreSame(expected, actual);
        }

        class MockObject
        {
            public MockObject() {}

            public MockObject(int val) {}
        }
    }
}