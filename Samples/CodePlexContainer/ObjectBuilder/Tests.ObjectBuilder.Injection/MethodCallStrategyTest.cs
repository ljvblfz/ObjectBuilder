using Xunit;

namespace ObjectBuilder
{
    public class MethodCallStrategyTest
    {
        [Test]
        public void ExecutesMethodsInPolicy()
        {
            Spy.Executed = false;
            MockBuilderContext context = new MockBuilderContext();
            MethodCallStrategy strategy = new MethodCallStrategy();
            MethodCallPolicy policy = new MethodCallPolicy();
            policy.Methods.Add(new ReflectionMethodCallInfo(typeof(Spy).GetMethod("InjectionMethod")));
            context.Policies.Set<IMethodCallPolicy>(policy, typeof(Spy));

            strategy.BuildUp(context, typeof(Spy), new Spy());

            Assert.True(Spy.Executed);
        }

        [Test]
        public void NoInstance()
        {
            Spy.Executed = false;
            MockBuilderContext context = new MockBuilderContext();
            MethodCallStrategy strategy = new MethodCallStrategy();
            MethodCallPolicy policy = new MethodCallPolicy();
            policy.Methods.Add(new ReflectionMethodCallInfo(typeof(Spy).GetMethod("InjectionMethod")));
            context.Policies.Set<IMethodCallPolicy>(policy, typeof(Spy));

            strategy.BuildUp(context, typeof(Spy), null);

            Assert.False(Spy.Executed);
        }

        internal class Spy
        {
            public static bool Executed;

            public void InjectionMethod()
            {
                Executed = true;
            }
        }
    }
}