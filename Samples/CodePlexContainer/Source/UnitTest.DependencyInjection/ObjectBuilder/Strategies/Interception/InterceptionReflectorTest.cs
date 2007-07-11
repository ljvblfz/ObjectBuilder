using System;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class InterceptionReflectorTest
    {
        [TestFixture]
        public class InterceptionViaInterface
        {
            [Test]
            public void NonPublicInterfaceNotCompatible()
            {
                PolicyList policies = new PolicyList();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        InterceptionReflector.Reflect<INonPublicInterface, NonPublicInterface>(null, policies, new StubObjectFactory());
                    });
            }

            [Test]
            public void RequestingToBuildNonInterfaceMeansMethodsNotIntercepted()
            {
                PolicyList policies = new PolicyList();

                InterceptionReflector.Reflect<OneMethod>(null, policies, new StubObjectFactory());

                Assert.Equal(0, policies.Count);
            }

            [Test]
            public void RequestingToBuildInterface1WillNotInterceptedInterface2Methods()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(IInterface1).GetMethod("InterceptedMethod1");

                InterceptionReflector.Reflect<IInterface1, TwoInterfaceClass>(null, policies, new StubObjectFactory());
                InterfaceInterceptionPolicy policy = policies.Get<InterfaceInterceptionPolicy>(typeof(TwoInterfaceClass), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void OneInterceptedMethod()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(IOneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<IOneMethod, OneMethod>("foo", policies, new StubObjectFactory());
                InterfaceInterceptionPolicy policy = policies.Get<InterfaceInterceptionPolicy>(typeof(OneMethod), "foo");

                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void TwoInterceptedMethods()
            {
                PolicyList policies = new PolicyList();
                MethodBase method1 = typeof(ITwoMethods).GetMethod("InterceptedMethod1");
                MethodBase method2 = typeof(ITwoMethods).GetMethod("InterceptedMethod2");

                InterceptionReflector.Reflect<ITwoMethods, TwoMethods>(null, policies, new StubObjectFactory());
                InterfaceInterceptionPolicy policy = policies.Get<InterfaceInterceptionPolicy>(typeof(TwoMethods), null);

                Assert.Equal(2, policy.Count);
                Assert.Equal(1, policy[method1].Count);
                Assert.IsType<RecordingHandler>(policy[method1][0]);
                Assert.Equal(1, policy[method2].Count);
                Assert.IsType<RecordingHandler>(policy[method2][0]);
            }

            [Test]
            public void TwoAttributesOnOneMethod()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(IOneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<IOneMethod, OneMethodTwoAttributes>(null, policies, new StubObjectFactory());
                InterfaceInterceptionPolicy policy = policies.Get<InterfaceInterceptionPolicy>(typeof(OneMethodTwoAttributes), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            [Test]
            public void InterceptionIsInherited()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(IOneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<IOneMethod, DerivedOneMethod>(null, policies, new StubObjectFactory());
                InterfaceInterceptionPolicy policy = policies.Get<InterfaceInterceptionPolicy>(typeof(DerivedOneMethod), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void CanAddHandlersInInheritedClass()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(IOneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<IOneMethod, DerivedWithAddedIntercepts>(null, policies, new StubObjectFactory());
                InterfaceInterceptionPolicy policy = policies.Get<InterfaceInterceptionPolicy>(typeof(DerivedWithAddedIntercepts), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            // Helpers

            internal interface INonPublicInterface
            {
                void InterceptedMethod();
            }

            internal class NonPublicInterface : INonPublicInterface
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod() {}
            }

            public interface IOneMethod
            {
                void InterceptedMethod();
            }

            internal class OneMethod : IOneMethod
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod() {}

                public void NonInterceptedMethod() {}
            }

            public interface ITwoMethods
            {
                void InterceptedMethod1();
                void InterceptedMethod2();
            }

            internal class TwoMethods : ITwoMethods
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod1() {}

                [InterfaceIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod2() {}
            }

            internal class OneMethodTwoAttributes : IOneMethod
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                [InterfaceIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod() {}
            }

            internal class DerivedOneMethod : OneMethod {}

            internal class DerivedWithAddedIntercepts : OneMethod
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                public override void InterceptedMethod() {}
            }

            public interface IInterface1
            {
                void InterceptedMethod1();
            }

            public interface IInterface2
            {
                void InterceptedMethod2();
            }

            internal class TwoInterfaceClass : IInterface1, IInterface2
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod1() {}

                [InterfaceIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod2() {}
            }
        }

        [TestFixture]
        public class InterceptionViaRemoting
        {
            [Test]
            public void NonMBROTypeIncompatibleWithRemoting()
            {
                PolicyList policies = new PolicyList();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        InterceptionReflector.Reflect<NonMBRO>(null, policies, new StubObjectFactory());
                    });
            }

            [Test]
            public void OneInterceptedMethod()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(OneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<OneMethod>("foo", policies, new StubObjectFactory());
                RemotingInterceptionPolicy policy = policies.Get<RemotingInterceptionPolicy>(typeof(OneMethod), "foo");

                Assert.NotNull(policy);
                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void TwoInterceptedMethods()
            {
                PolicyList policies = new PolicyList();
                MethodBase method1 = typeof(TwoMethods).GetMethod("InterceptedMethod1");
                MethodBase method2 = typeof(TwoMethods).GetMethod("InterceptedMethod2");

                InterceptionReflector.Reflect<TwoMethods>(null, policies, new StubObjectFactory());
                RemotingInterceptionPolicy policy = policies.Get<RemotingInterceptionPolicy>(typeof(TwoMethods), null);

                Assert.Equal(2, policy.Count);
                Assert.Equal(1, policy[method1].Count);
                Assert.IsType<RecordingHandler>(policy[method1][0]);
                Assert.Equal(1, policy[method2].Count);
                Assert.IsType<RecordingHandler>(policy[method2][0]);
            }

            [Test]
            public void TwoAttributesOnOneMethod()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(OneMethodTwoAttributes).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<OneMethodTwoAttributes>(null, policies, new StubObjectFactory());
                RemotingInterceptionPolicy policy = policies.Get<RemotingInterceptionPolicy>(typeof(OneMethodTwoAttributes), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            [Test]
            public void InterceptionIsInherited()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(DerivedOneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<DerivedOneMethod>(null, policies, new StubObjectFactory());
                RemotingInterceptionPolicy policy = policies.Get<RemotingInterceptionPolicy>(typeof(DerivedOneMethod), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void CanAddHandlersInInheritedClass()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(DerivedWithAddedIntercepts).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<DerivedWithAddedIntercepts>(null, policies, new StubObjectFactory());
                RemotingInterceptionPolicy policy = policies.Get<RemotingInterceptionPolicy>(typeof(DerivedWithAddedIntercepts), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            // Helpers

            internal class OneMethod : MarshalByRefObject
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod() {}

                public void NonInterceptedMethod() {}
            }

            internal class TwoMethods : MarshalByRefObject
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod1() {}

                [RemotingIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod2() {}
            }

            internal class OneMethodTwoAttributes : MarshalByRefObject
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                [RemotingIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod() {}
            }

            internal class NonMBRO
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod() {}
            }

            internal class DerivedOneMethod : OneMethod {}

            internal class DerivedWithAddedIntercepts : OneMethod
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                public override void InterceptedMethod() {}
            }
        }

        [TestFixture]
        public class InterceptionViaVirtualMethod
        {
            [Test]
            public void NonPublicTypesNotCompatible()
            {
                PolicyList policies = new PolicyList();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        InterceptionReflector.Reflect<InternalClass>(null, policies, new StubObjectFactory());
                    });
            }

            [Test]
            public void SealedTypeNotCompatible()
            {
                PolicyList policies = new PolicyList();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        InterceptionReflector.Reflect<SealedClass>(null, policies, new StubObjectFactory());
                    });
            }

            [Test]
            public void NonVirtualMethodNotCompatible()
            {
                PolicyList policies = new PolicyList();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        InterceptionReflector.Reflect<NonVirtualMethod>(null, policies, new StubObjectFactory());
                    });
            }

            [Test]
            public void VirtualSealedMethodNotCompatible()
            {
                PolicyList policies = new PolicyList();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        InterceptionReflector.Reflect<VirtualSealed>(null, policies, new StubObjectFactory());
                    });
            }

            [Test]
            public void OneInterceptedMethod()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(OneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<OneMethod>("foo", policies, new StubObjectFactory());
                VirtualInterceptionPolicy policy = policies.Get<VirtualInterceptionPolicy>(typeof(OneMethod), "foo");

                Assert.NotNull(policy);
                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void TwoInterceptedMethods()
            {
                PolicyList policies = new PolicyList();
                MethodBase method1 = typeof(TwoMethods).GetMethod("InterceptedMethod1");
                MethodBase method2 = typeof(TwoMethods).GetMethod("InterceptedMethod2");

                InterceptionReflector.Reflect<TwoMethods>(null, policies, new StubObjectFactory());
                VirtualInterceptionPolicy policy = policies.Get<VirtualInterceptionPolicy>(typeof(TwoMethods), null);

                Assert.Equal(2, policy.Count);
                Assert.Equal(1, policy[method1].Count);
                Assert.IsType<RecordingHandler>(policy[method1][0]);
                Assert.Equal(1, policy[method2].Count);
                Assert.IsType<RecordingHandler>(policy[method2][0]);
            }

            [Test]
            public void TwoAttributesOnOneMethod()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(OneMethodTwoAttributes).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<OneMethodTwoAttributes>(null, policies, new StubObjectFactory());
                VirtualInterceptionPolicy policy = policies.Get<VirtualInterceptionPolicy>(typeof(OneMethodTwoAttributes), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            [Test]
            public void InterceptionIsInherited()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(DerivedOneMethod).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<DerivedOneMethod>(null, policies, new StubObjectFactory());
                VirtualInterceptionPolicy policy = policies.Get<VirtualInterceptionPolicy>(typeof(DerivedOneMethod), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(1, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
            }

            [Test]
            public void CanAddHandlersInInheritedClass()
            {
                PolicyList policies = new PolicyList();
                MethodBase method = typeof(DerivedWithAddedIntercepts).GetMethod("InterceptedMethod");

                InterceptionReflector.Reflect<DerivedWithAddedIntercepts>(null, policies, new StubObjectFactory());
                VirtualInterceptionPolicy policy = policies.Get<VirtualInterceptionPolicy>(typeof(DerivedWithAddedIntercepts), null);

                Assert.Equal(1, policy.Count);
                Assert.Equal(2, policy[method].Count);
                Assert.IsType<RecordingHandler>(policy[method][0]);
                Assert.IsType<RecordingHandler>(policy[method][1]);
            }

            // Helpers

            internal class InternalClass
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod() {}
            }

            public class SealedClassBase
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod() {}
            }

            public sealed class SealedClass : SealedClassBase {}

            public class OneMethod
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod() {}

                public void NonInterceptedMethod() {}
            }

            public class TwoMethods
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod1() {}

                [VirtualIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod2() {}
            }

            public class OneMethodTwoAttributes
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                [VirtualIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod() {}
            }

            public class NonVirtualMethod
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod() {}
            }

            public class VirtualSealed : OneMethod
            {
                public override sealed void InterceptedMethod() {}
            }

            public class DerivedOneMethod : OneMethod {}

            public class DerivedWithAddedIntercepts : OneMethod
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                public override void InterceptedMethod() {}
            }
        }

        [TestFixture]
        public class MixedInterception
        {
            [Test]
            public void CanMixInterceptionTypes()
            {
                PolicyList policies = new PolicyList();

                InterceptionReflector.Reflect<IFoo, FooBar>(null, policies, new StubObjectFactory());

                Assert.Equal(1, policies.Get<VirtualInterceptionPolicy>(typeof(FooBar), null).Count);
                Assert.Equal(1, policies.Get<InterfaceInterceptionPolicy>(typeof(FooBar), null).Count);
                Assert.Equal(1, policies.Get<RemotingInterceptionPolicy>(typeof(FooBar), null).Count);
            }

            public interface IFoo
            {
                void Foo();
            }

            public class FooBar : MarshalByRefObject, IFoo
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                [VirtualIntercept(typeof(RecordingHandler))]
                [InterfaceIntercept(typeof(RecordingHandler))]
                public virtual void Foo() {}
            }
        }

        [TestFixture]
        public class NoInterceptionType
        {
            [Test]
            public void ClassWithNoDecorations()
            {
                PolicyList policies = new PolicyList();

                InterceptionReflector.Reflect<object>(null, policies, new StubObjectFactory());

                Assert.Equal(0, policies.Count);
            }
        }
    }
}