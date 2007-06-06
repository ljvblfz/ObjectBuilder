using System;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection
{
    [TestFixture]
    public class MethodInvocationTest
    {
        [Test]
        public void ShouldBeAbleToChangeInputs()
        {
            MethodBase methodBase = typeof(InvocationTarget).GetMethod("FirstTarget");
            InvocationTarget target = new InvocationTarget();
            IMethodInvocation invocation = new MethodInvocation(target, methodBase, new object[] { 1, "two" });

            Assert.Equal<object>(1, invocation.Inputs["one"]);
            invocation.Inputs["one"] = 42;
            Assert.Equal<object>(42, invocation.Inputs["one"]);
        }

        // Helpers

        internal class InvocationTarget : MarshalByRefObject
        {
            public string FirstTarget(int one,
                                      string two)
            {
                return "Boo!";
            }
        }
    }
}