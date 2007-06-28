using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class VirtualMethodInterceptorTest
    {
        [Test]
        public void NoReturnValueNoParameters()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("NoReturnValueNoParameters");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            result.NoReturnValueNoParameters();

            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void ReturnsClassType()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("ReturnsClassType");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            object retValue = result.ReturnsClassType();

            Assert.Same(SpyVirtualClass.ObjectReturn, retValue);
            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void ReturnsValueType()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("ReturnsValueType");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            int retValue = result.ReturnsValueType();

            Assert.Equal(SpyVirtualClass.ValueReturn, retValue);
            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void SingleParameter()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("SingleParameter");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            int retValue = result.SingleParameter(21);

            Assert.Equal(21 * 2, retValue);
            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void TwoParameters()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("TwoParameters");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            string retValue = result.TwoParameters(42, "Hello ");

            Assert.Equal("Hello 42", retValue);
            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void TwentyParameters()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("TwentyParameters");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            int retValue = result.TwentyParameters(12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10);

            Assert.Equal(120, retValue);
            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void OutParameters()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("OutParameters");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            string outReference;
            int outValue;
            result.OutParameters(out outReference, out outValue);

            Assert.Equal(SpyVirtualClass.OutReference, outReference);
            Assert.Equal(SpyVirtualClass.OutValue, outValue);
            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void RefClassParameter()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("RefClassParameter");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            string retValue = "Hello, ";
            result.RefClassParameter(ref retValue);

            Assert.Equal(SpyVirtualClass.OutReference, retValue);
            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void Exception()
        {
            Recorder.Records.Clear();
            SpyVirtualClass rawObject = new SpyVirtualClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("Exception");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyVirtualClass result = VirtualMethodInterceptor<SpyVirtualClass>.Wrap(rawObject, dictionary);
            Assert.Throws<ArgumentException>(delegate
                                             {
                                                 result.Exception();
                                             });

            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        // Out value type
        // Ref value type
        // Sealed class
        // Non-virtual method
        // Property getter/setter
    }

    class Derived : SpyVirtualClass
    {
        readonly VirtualMethodProxy proxy;
        readonly object target;

        public Derived(VirtualMethodProxy proxy,
                       object target)
        {
            this.proxy = proxy;
            this.target = target;
        }

        public override void Exception()
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
        }

        public override void NoReturnValueNoParameters()
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
        }

        public override void OutParameters(out string outReference,
                                           out int outValue)
        {
            outReference = default(string);
            outValue = default(int);

            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            object[] arguments = new object[] { outReference, outValue };
            proxy.Invoke(target, currentMethod.GetBaseDefinition(), arguments);

            outReference = (string)arguments[0];
            outValue = (int)arguments[1];
        }

        public override void RefClassParameter(ref string value)
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            object[] arguments = new object[] { value };
            proxy.Invoke(target, currentMethod.GetBaseDefinition(), arguments);

            value = (string)arguments[0];
        }

        public override object ReturnsClassType()
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            return proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
        }

        public override int ReturnsValueType()
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            return (int)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
        }

        public override int SingleParameter(int x)
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            return (int)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[] { x });
        }

        public override int TwentyParameters(int p0,
                                             int p1,
                                             int p2,
                                             int p3,
                                             int p4,
                                             int p5,
                                             int p6,
                                             int p7,
                                             int p8,
                                             int p9,
                                             int p10,
                                             int p11,
                                             int p12,
                                             int p13,
                                             int p14,
                                             int p15,
                                             int p16,
                                             int p17,
                                             int p18,
                                             int p19)
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            return (int)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19 });
        }

        public override string TwoParameters(int x,
                                             string y)
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            return (string)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[] { x, y });
        }
    }

    public class SpyVirtualClass
    {
        public const string OutReference = "Hello, world!";
        public const int OutValue = 2112;
        public const int ValueReturn = 42;
        public static object ObjectReturn = new object();

        public virtual void Exception()
        {
            Recorder.Records.Add("In Method");
            throw new ArgumentException();
        }

        public virtual void NoReturnValueNoParameters()
        {
            Recorder.Records.Add("In Method");
        }

        public virtual void OutParameters(out string outReference,
                                          out int outValue)
        {
            Recorder.Records.Add("In Method");
            outReference = OutReference;
            outValue = OutValue;
        }

        public virtual void RefClassParameter(ref string value)
        {
            Recorder.Records.Add("In Method");
            value += "world!";
        }

        public virtual object ReturnsClassType()
        {
            Recorder.Records.Add("In Method");
            return ObjectReturn;
        }

        public virtual int ReturnsValueType()
        {
            Recorder.Records.Add("In Method");
            return ValueReturn;
        }

        public virtual int SingleParameter(int x)
        {
            Recorder.Records.Add("In Method");
            return x * 2;
        }

        public virtual int TwentyParameters(int p0,
                                            int p1,
                                            int p2,
                                            int p3,
                                            int p4,
                                            int p5,
                                            int p6,
                                            int p7,
                                            int p8,
                                            int p9,
                                            int p10,
                                            int p11,
                                            int p12,
                                            int p13,
                                            int p14,
                                            int p15,
                                            int p16,
                                            int p17,
                                            int p18,
                                            int p19)
        {
            Recorder.Records.Add("In Method");
            return p0 * p19;
        }

        public virtual string TwoParameters(int x,
                                            string y)
        {
            Recorder.Records.Add("In Method");
            return y + x;
        }
    }
}