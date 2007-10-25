using System;
using System.Collections.Generic;
using Xunit;

namespace ObjectBuilder
{
    public class ActivatorCreationPolicyTest
    {
        [Fact]
        public void CreatesObjectAndPassesValue()
        {
            MockBuilderContext context = new MockBuilderContext();
            ActivatorCreationPolicy policy = new ActivatorCreationPolicy(Params(42));

            Dummy result = (Dummy)policy.Create(context, typeof(Dummy));

            Assert.NotNull(result);
            Assert.Equal(42, result.val);
        }

        [Fact]
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

        [Fact]
        public void NullContext()
        {
            ActivatorCreationPolicy policy = new ActivatorCreationPolicy();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    policy.Create(null, typeof(Dummy));
                });
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