using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class ActivatorCreationPolicyTest
    {
        [Test]
        public void NullContext()
        {
            ActivatorCreationPolicy policy = new ActivatorCreationPolicy();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    policy.Create(null, typeof(Dummy));
                });
        }

        [Test]
        public void NoMatchingConstructor()
        {
            MockBuilderContext context = new MockBuilderContext();
            ActivatorCreationPolicy policy = new ActivatorCreationPolicy(Params("foo"));

            Assert.Throws<MissingMethodException>(
                delegate
                {
                    policy.Create(context, typeof(Dummy));
                });
        }

        [Test]
        public void CreatesObjectAndPassesValue()
        {
            MockBuilderContext context = new MockBuilderContext();
            ActivatorCreationPolicy policy = new ActivatorCreationPolicy(Params(42));

            Dummy result = (Dummy)policy.Create(context, typeof(Dummy));

            Assert.NotNull(result);
            Assert.Equal(42, result.val);
        }

        static IEnumerable<IParameter> Params(params object[] parameters)
        {
            foreach (object parameter in parameters)
                yield return new ValueParameter(parameter.GetType(), parameter);
        }

        internal class Dummy
        {
            public readonly int val;

            public Dummy(int val)
            {
                this.val = val;
            }
        }
    }
}