using System;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class InterceptionReflectorTest
    {
        [TestFixture]
        public class InterceptionViaRemoting
        {
            [Test]
            public void NoInterceptedMethods()
            {
                IInterceptionPolicy policy = InterceptionReflector.Reflect<ZeroMethods>(new StubObjectFactory());

                Assert.Null(policy);
            }

            [Test]
            public void OneInterceptedMethod()
            {
                MethodBase method = typeof(OneMethod).GetMethod("InterceptedMethod");

                IInterceptionPolicy policy = InterceptionReflector.Reflect<OneMethod>(new StubObjectFactory());

                Assert.IsType<RemotingInterceptionPolicy>(policy);
                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void TwoInterceptedMethods()
            {
                MethodBase method1 = typeof(TwoMethods).GetMethod("InterceptedMethod1");
                MethodBase method2 = typeof(TwoMethods).GetMethod("InterceptedMethod2");

                IInterceptionPolicy policy = InterceptionReflector.Reflect<TwoMethods>(new StubObjectFactory());

                Assert.IsType<RemotingInterceptionPolicy>(policy);
                Assert.Equal(2, policy.Count);
                Assert.Equal(1, policy[method1].Count);
                Assert.IsType<RecordingHandler>(policy[method1][0]);
                Assert.Equal(1, policy[method2].Count);
                Assert.IsType<RecordingHandler>(policy[method2][0]);
            }

            [Test]
            public void TwoAttributesOnOneMethod()
            {
                MethodBase method = typeof(OneMethodTwoAttributes).GetMethod("InterceptedMethod");

                IInterceptionPolicy policy = InterceptionReflector.Reflect<OneMethodTwoAttributes>(new StubObjectFactory());

                Assert.IsType<RemotingInterceptionPolicy>(policy);
                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            [Test]
            public void ConcreteTypeIncompatibleWithRemoting()
            {
                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        InterceptionReflector.Reflect<NonMBRO>(new StubObjectFactory());
                    });
            }

            [Test]
            public void CanRequestInterfaceForNonMBROClass()
            {
                MethodBase method = typeof(NonMBRO).GetMethod("InterceptedMethod");

                IInterceptionPolicy policy = InterceptionReflector.Reflect<INonMBRO, NonMBRO>(new StubObjectFactory());

                Assert.IsType<RemotingInterceptionPolicy>(policy);
                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void InterceptionIsInherited()
            {
                MethodBase method = typeof(DerivedOneMethod).GetMethod("InterceptedMethod");

                IInterceptionPolicy policy = InterceptionReflector.Reflect<DerivedOneMethod>(new StubObjectFactory());

                Assert.IsType<RemotingInterceptionPolicy>(policy);
                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void CanAddHandlersInInheritedClass()
            {
                MethodBase method = typeof(DerivedWithAddedIntercepts).GetMethod("InterceptedMethod");

                IInterceptionPolicy policy = InterceptionReflector.Reflect<DerivedWithAddedIntercepts>(new StubObjectFactory());

                Assert.IsType<RemotingInterceptionPolicy>(policy);
                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            // Helpers

            [RemotingInterception]
            internal class ZeroMethods : MarshalByRefObject {}

            [RemotingInterception]
            internal class OneMethod : MarshalByRefObject
            {
                [Intercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod() {}

                public void NonInterceptedMethod() {}
            }

            [RemotingInterception]
            internal class TwoMethods : MarshalByRefObject
            {
                [Intercept(typeof(RecordingHandler))]
                public void InterceptedMethod1() {}

                [Intercept(typeof(RecordingHandler))]
                public void InterceptedMethod2() {}
            }

            [RemotingInterception]
            internal class OneMethodTwoAttributes : MarshalByRefObject
            {
                [Intercept(typeof(RecordingHandler))]
                [Intercept(typeof(RecordingHandler))]
                public void InterceptedMethod() {}
            }

            internal interface INonMBRO
            {
                void InterceptedMethod();
            }

            [RemotingInterception]
            internal class NonMBRO : INonMBRO
            {
                [Intercept(typeof(RecordingHandler))]
                public void InterceptedMethod() {}
            }

            internal class DerivedOneMethod : OneMethod {}

            internal class DerivedWithAddedIntercepts : OneMethod
            {
                [Intercept(typeof(RecordingHandler))]
                public override void InterceptedMethod() {}
            }
        }

        [TestFixture]
        public class NoInterceptionType
        {
            [Test]
            public void ClassWithNoDecorationsReturnsNullPolicy()
            {
                IInterceptionPolicy policy = InterceptionReflector.Reflect<object>(new StubObjectFactory());

                Assert.Null(policy);
            }
        }
    }
}