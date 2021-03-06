using Xunit;

namespace ObjectBuilder
{
    public class MethodReflectionStrategyTest
    {
        [Fact]
        public void DecoratedMethod()
        {
            MockBuilderContext context = new MockBuilderContext();
            MethodReflectionStrategy strategy = new MethodReflectionStrategy();

            strategy.BuildUp(context, typeof(Decorated), null);

            IMethodCallPolicy policy = context.Policies.Get<IMethodCallPolicy>(typeof(Decorated));
            Assert.NotNull(policy);
            Assert.NotEmpty(policy.Methods);
            foreach (ReflectionMethodCallInfo method in policy.Methods)
                Assert.Equal(typeof(Decorated).GetMethod("Method"), method.Method);
        }

        [Fact]
        public void NoDecoratedMethods()
        {
            MockBuilderContext context = new MockBuilderContext();
            MethodReflectionStrategy strategy = new MethodReflectionStrategy();

            strategy.BuildUp(context, typeof(object), null);

            IMethodCallPolicy policy = context.Policies.Get<IMethodCallPolicy>(typeof(object));
            Assert.Null(policy);
        }

        internal class Decorated
        {
            [InjectionMethod]
            public void Method() {}

            public void UndecoratedMethod() {}
        }
    }
}