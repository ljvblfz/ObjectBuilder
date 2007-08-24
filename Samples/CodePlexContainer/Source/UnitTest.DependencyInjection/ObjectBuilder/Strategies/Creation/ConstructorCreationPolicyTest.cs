using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class ConstructorCreationPolicyTest
    {
        [Test]
        public void NullConstructor()
        {
            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    new ConstructorCreationPolicy(null);
                });
        }

        [Test]
        public void NullContext()
        {
            ConstructorInfo constructor = typeof(Dummy).GetConstructor(new Type[] { typeof(int) });
            ConstructorCreationPolicy policy = new ConstructorCreationPolicy(constructor);

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    policy.Create(null, typeof(Dummy));
                });
        }

        [Test]
        public void NonMatchingParameterCount()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorInfo constructor = typeof(Dummy).GetConstructor(new Type[] { typeof(int) });
            ConstructorCreationPolicy policy = new ConstructorCreationPolicy(constructor);

            Assert.Throws<TargetParameterCountException>(
                delegate
                {
                    policy.Create(context, typeof(Dummy));
                });
        }

        [Test]
        public void NonMatchingParameterTypes()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorInfo constructor = typeof(Dummy).GetConstructor(new Type[] { typeof(int) });
            ConstructorCreationPolicy policy = new ConstructorCreationPolicy(constructor, Params("foo"));

            Assert.Throws<ArgumentException>(
                delegate
                {
                    policy.Create(context, typeof(Dummy));
                });
        }

        [Test]
        public void CreatesObjectAndPassesValue()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorInfo constructor = typeof(Dummy).GetConstructor(new Type[] { typeof(int) });
            ConstructorCreationPolicy policy = new ConstructorCreationPolicy(constructor, Params(42));

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