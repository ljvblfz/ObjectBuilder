using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace ObjectBuilder
{
    public class ReflectionStrategyTest
    {
        [Test]
        public void CallsAddParametersToPolicy()
        {
            TestableReflectionStrategy strategy = new TestableReflectionStrategy();
            MockBuilderContext context = new MockBuilderContext();
            MethodInfo method = typeof(Dummy).GetMethod("Method1");
            strategy.GetMembers__Result.Add(new MethodMemberInfo<MethodInfo>(method));

            strategy.BuildUp(context, "foo", null);

            Assert.Same(context, strategy.AddParametersToPolicy_Context);
            Assert.Same("foo", strategy.AddParametersToPolicy_BuildKey);
            Assert.Same(method, strategy.AddParametersToPolicy_Member);
        }

        [Test]
        public void CallsGetMembers()
        {
            TestableReflectionStrategy strategy = new TestableReflectionStrategy();
            MockBuilderContext context = new MockBuilderContext();
            object existing = new object();

            strategy.BuildUp(context, "foo", existing);

            Assert.Same(context, strategy.GetMembers_Context);
            Assert.Same("foo", strategy.GetMembers_BuildKey);
            Assert.Same(existing, strategy.GetMembers_Existing);
        }

        [Test]
        public void CallsMemberRequiresProcessing()
        {
            TestableReflectionStrategy strategy = new TestableReflectionStrategy();
            MockBuilderContext context = new MockBuilderContext();
            MethodInfo method1 = typeof(Dummy).GetMethod("Method1");
            MethodInfo method2 = typeof(Dummy).GetMethod("Method2");
            strategy.GetMembers__Result.Add(new MethodMemberInfo<MethodInfo>(method1));
            strategy.GetMembers__Result.Add(new MethodMemberInfo<MethodInfo>(method2));

            strategy.BuildUp(context, "foo", null);

            Assert.Contains(method1, strategy.MemberRequiresProcessing_Members);
            Assert.Contains(method2, strategy.MemberRequiresProcessing_Members);
        }

        [Test]
        public void DefaultParameterBehaviorIsBuildDependencyByType()
        {
            TestableReflectionStrategy strategy = new TestableReflectionStrategy();
            MockBuilderContext context = new MockBuilderContext();
            MethodInfo method = typeof(Dummy).GetMethod("Method1");
            strategy.GetMembers__Result.Add(new MethodMemberInfo<MethodInfo>(method));

            strategy.BuildUp(context, "foo", null);

            IParameter parameter = strategy.AddParametersToPolicy_Parameters[0];
            DependencyParameter dependency = Assert.IsType<DependencyParameter>(parameter);
            Assert.Equal<object>(typeof(int), dependency.BuildKey);
            Assert.Equal(NotPresentBehavior.Build, dependency.NotPresentBehavior);
        }

        [Test]
        public void MultipleAttributesNotAllowed()
        {
            TestableReflectionStrategy strategy = new TestableReflectionStrategy();
            MockBuilderContext context = new MockBuilderContext();
            MethodInfo method = typeof(Dummy).GetMethod("Method3");
            strategy.GetMembers__Result.Add(new MethodMemberInfo<MethodInfo>(method));

            Assert.Throws<InvalidAttributeException>(
                delegate
                {
                    strategy.BuildUp(context, "foo", null);
                });
        }

        [Test]
        public void UsesAttributeInforationWhenPresent()
        {
            TestableReflectionStrategy strategy = new TestableReflectionStrategy();
            MockBuilderContext context = new MockBuilderContext();
            MethodInfo method = typeof(Dummy).GetMethod("Method2");
            strategy.GetMembers__Result.Add(new MethodMemberInfo<MethodInfo>(method));

            strategy.BuildUp(context, "foo", null);

            IParameter parameter = strategy.AddParametersToPolicy_Parameters[0];
            DependencyParameter dependency = Assert.IsType<DependencyParameter>(parameter);
            Assert.Equal<object>("bar", dependency.BuildKey);
            Assert.Equal(NotPresentBehavior.Throw, dependency.NotPresentBehavior);
        }

        internal class Dummy
        {
            public void Method1(int x) {}

            public void Method2([Dependency("bar", NotPresentBehavior = NotPresentBehavior.Throw)] int x) {}

            public void Method3([Dependency] [DummyParameter] int x) {}
        }

        internal class DummyParameterAttribute : ParameterAttribute
        {
            public override IParameter CreateParameter(Type annotatedMemberType)
            {
                return new ValueParameter<string>("baz");
            }
        }

        internal class TestableReflectionStrategy : ReflectionStrategy<MethodInfo>
        {
            public object AddParametersToPolicy_BuildKey;
            public IBuilderContext AddParametersToPolicy_Context;
            public MethodInfo AddParametersToPolicy_Member;
            public List<IParameter> AddParametersToPolicy_Parameters = new List<IParameter>();
            public List<IMemberInfo<MethodInfo>> GetMembers__Result = new List<IMemberInfo<MethodInfo>>();
            public object GetMembers_BuildKey;
            public IBuilderContext GetMembers_Context;
            public object GetMembers_Existing;
            public List<MethodInfo> MemberRequiresProcessing_Members = new List<MethodInfo>();

            protected override void AddParametersToPolicy(IBuilderContext context,
                                                          object buildKey,
                                                          IMemberInfo<MethodInfo> member,
                                                          IEnumerable<IParameter> parameters)
            {
                AddParametersToPolicy_Context = context;
                AddParametersToPolicy_BuildKey = buildKey;
                AddParametersToPolicy_Member = member.MemberInfo;
                AddParametersToPolicy_Parameters.AddRange(parameters);
            }

            protected override IEnumerable<IMemberInfo<MethodInfo>> GetMembers(IBuilderContext context,
                                                                               object buildKey,
                                                                               object existing)
            {
                GetMembers_Context = context;
                GetMembers_BuildKey = buildKey;
                GetMembers_Existing = existing;

                return GetMembers__Result;
            }

            protected override bool MemberRequiresProcessing(IMemberInfo<MethodInfo> member)
            {
                MemberRequiresProcessing_Members.Add(member.MemberInfo);

                return true;
            }
        }
    }
}