using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodInterceptorTest
    {
        [TestFixture]
        public class Errors
        {
            [Test]
            public void CannotInterceptSealedClass()
            {
                SealedClass rawObject = new SealedClass();
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();

                Assert.Throws<TypeLoadException>(
                    delegate
                    {
                        VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                    });
            }

            [Test]
            public void CannotInterceptNonPublicClass()
            {
                PrivateClass rawObject = new PrivateClass();
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();

                Assert.Throws<TypeLoadException>(
                    delegate
                    {
                        VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                    });
            }

            [Test]
            public void CannotInterceptNonVirtualMethod()
            {
                NonVirtual rawObject = new NonVirtual();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("NonVirtualMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                    });
            }

            [Test]
            public void CannotInterceptVirtualSealedMethod()
            {
                DerivedClass rawObject = new DerivedClass();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("VirtualMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                    });
            }

            [Test]
            public void HandlerFromWrongType()
            {
                DerivedClass rawObject = new DerivedClass();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(NonVirtual).GetMethod("NonVirtualMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                    });
            }

            [Test]
            public void NonPublicMethod()
            {
                DerivedClass rawObject = new DerivedClass();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("NonPublicMethod", BindingFlags.NonPublic | BindingFlags.Instance);
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                    });
            }

            public sealed class SealedClass {}

            class PrivateClass {}

            public class NonVirtual
            {
                public void NonVirtualMethod() {}
            }

            public class BaseClass
            {
                public virtual void VirtualMethod() {}
            }

            public class DerivedClass : BaseClass
            {
                internal virtual void NonPublicMethod() {}
                public override sealed void VirtualMethod() {}
            }
        }

        [TestFixture]
        public class InParameters
        {
            [Test]
            public void OneParameter()
            {
                Recorder.Records.Clear();
                SpyIn rawObject = new SpyIn();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("OneParameter");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyIn result = (SpyIn)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                int retValue = result.OneParameter(21);

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
                SpyIn rawObject = new SpyIn();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("TwoParameters");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyIn result = (SpyIn)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
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
                SpyIn rawObject = new SpyIn();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("TwentyParameters");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyIn result = (SpyIn)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                int retValue = result.TwentyParameters(12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10);

                Assert.Equal(120, retValue);
                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            public class SpyIn
            {
                public virtual int OneParameter(int x)
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

        [TestFixture]
        public class OutParameters
        {
            [Test]
            public void OutReferenceType()
            {
                SpyOut rawObject = new SpyOut();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("OutReferenceType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = (SpyOut)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                string outReference;
                result.OutReferenceType(out outReference);

                Assert.Equal("Hello, world!", outReference);
            }

            [Test]
            public void OutInt16()
            {
                SpyOut rawObject = new SpyOut();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("OutInt16");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = (SpyOut)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                short outValue;
                result.OutInt16(out outValue);

                Assert.Equal(short.MaxValue, outValue);
            }

            [Test]
            public void OutInt32()
            {
                SpyOut rawObject = new SpyOut();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("OutInt32");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = (SpyOut)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                int outValue;
                result.OutInt32(out outValue);

                Assert.Equal(int.MaxValue, outValue);
            }

            [Test]
            public void OutInt64()
            {
                SpyOut rawObject = new SpyOut();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("OutInt64");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = (SpyOut)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                long outValue;
                result.OutInt64(out outValue);

                Assert.Equal(long.MaxValue, outValue);
            }

            [Test]
            public void OutDouble()
            {
                SpyOut rawObject = new SpyOut();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("OutDouble");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = (SpyOut)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                double outValue;
                result.OutDouble(out outValue);

                Assert.Equal(double.MaxValue, outValue);
            }

            [Test]
            public void OutComplexValueType()
            {
                SpyOut rawObject = new SpyOut();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("OutComplexValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = (SpyOut)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                ComplexValueType outValue;
                result.OutComplexValueType(out outValue);

                Assert.Equal(byte.MaxValue, outValue.Byte);
                Assert.Equal('a', outValue.Char);
                Assert.Equal(decimal.MaxValue, outValue.Decimal);
                Assert.Equal(double.MaxValue, outValue.Double);
                Assert.Equal(float.MaxValue, outValue.Float);
                Assert.Equal(int.MaxValue, outValue.Int);
                Assert.Equal(long.MaxValue, outValue.Long);
                Assert.Equal(short.MaxValue, outValue.Short);
                Assert.Equal("Hello, world!", outValue.String);
                Assert.Equal(uint.MaxValue, outValue.UInt);
                Assert.Equal(ulong.MaxValue, outValue.ULong);
                Assert.Equal(ushort.MaxValue, outValue.UShort);
            }

            public class SpyOut
            {
                public virtual void OutChar(out char outValue)
                {
                    outValue = 'a';
                }

                public virtual void OutComplexValueType(out ComplexValueType outValueType)
                {
                    outValueType = new ComplexValueType();
                    outValueType.Byte = byte.MaxValue;
                    outValueType.Char = 'a';
                    outValueType.Decimal = decimal.MaxValue;
                    outValueType.Double = double.MaxValue;
                    outValueType.Float = float.MaxValue;
                    outValueType.Int = int.MaxValue;
                    outValueType.Long = long.MaxValue;
                    outValueType.Short = short.MaxValue;
                    outValueType.String = "Hello, world!";
                    outValueType.UInt = uint.MaxValue;
                    outValueType.ULong = ulong.MaxValue;
                    outValueType.UShort = ushort.MaxValue;
                }

                public virtual void OutDouble(out double outValue)
                {
                    outValue = double.MaxValue;
                }

                public virtual void OutInt16(out short outValue)
                {
                    outValue = short.MaxValue;
                }

                public virtual void OutInt32(out int outValue)
                {
                    outValue = int.MaxValue;
                }

                public virtual void OutInt64(out long outValue)
                {
                    outValue = long.MaxValue;
                }

                public virtual void OutReferenceType(out string outReference)
                {
                    outReference = "Hello, world!";
                }
            }
        }

        [TestFixture]
        public class RefParameters
        {
            [Test]
            public void RefClassParameter()
            {
                SpyRef rawObject = new SpyRef();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("RefClassParameter");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyRef result = (SpyRef)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                string refValue = "Hello, ";
                result.RefClassParameter(ref refValue);

                Assert.Equal("Hello, world!", refValue);
            }

            public class SpyRef
            {
                public virtual void RefClassParameter(ref string value)
                {
                    value += "world!";
                }
            }
        }

        [TestFixture]
        public class ReturnValues
        {
            [Test]
            public void NoReturnValue()
            {
                Recorder.Records.Clear();
                SpyReturn rawObject = new SpyReturn();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("NoReturnValue");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = (SpyReturn)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                result.NoReturnValue();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void ReturnsClassType()
            {
                SpyReturn rawObject = new SpyReturn();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("ReturnsClassType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = (SpyReturn)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                object retValue = result.ReturnsClassType();

                Assert.Same(SpyReturn.ObjectReturn, retValue);
            }

            [Test]
            public void ReturnsValueType()
            {
                SpyReturn rawObject = new SpyReturn();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("ReturnsValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = (SpyReturn)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                int retValue = result.ReturnsValueType();

                Assert.Equal(SpyReturn.ValueReturn, retValue);
            }

            [Test]
            public void Exception()
            {
                SpyReturn rawObject = new SpyReturn();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = rawObject.GetType().GetMethod("Exception");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = (SpyReturn)VirtualMethodInterceptor.Wrap(rawObject, dictionary);
                Assert.Throws<ArgumentException>(delegate
                                                 {
                                                     result.Exception();
                                                 });
            }

            public class SpyReturn
            {
                public const int ValueReturn = 42;
                public static object ObjectReturn = new object();

                public virtual void Exception()
                {
                    throw new ArgumentException();
                }

                public virtual void NoReturnValue()
                {
                    Recorder.Records.Add("In Method");
                }

                public virtual object ReturnsClassType()
                {
                    return ObjectReturn;
                }

                public virtual int ReturnsValueType()
                {
                    return ValueReturn;
                }
            }
        }
    }

    //class Derived : SpyVirtualClass
    //{
    //    readonly VirtualMethodProxy proxy;
    //    readonly object target;

    //    public Derived(VirtualMethodProxy proxy,
    //                   object target)
    //    {
    //        this.proxy = proxy;
    //        this.target = target;
    //    }

    //    public override void Exception()
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
    //    }

    //    public override void NoReturnValueNoParameters()
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
    //    }

    //    public override int OneParameter(int x)
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        return (int)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[] { x });
    //    }

    //    public override void OutComplexValueType(out ComplexValueType outValueType)
    //    {
    //        outValueType = default(ComplexValueType);

    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        object[] arguments = new object[] { outValueType };
    //        proxy.Invoke(target, currentMethod.GetBaseDefinition(), arguments);

    //        outValueType = (ComplexValueType)arguments[0];
    //    }

    //    public override void OutReferenceType(out string outReference,
    //                                       out int outValue)
    //    {
    //        outReference = default(string);
    //        outValue = default(int);

    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        object[] arguments = new object[] { outReference, outValue };
    //        proxy.Invoke(target, currentMethod.GetBaseDefinition(), arguments);

    //        outReference = (string)arguments[0];
    //        outValue = (int)arguments[1];
    //    }

    //    public override void RefClassParameter(ref string value)
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        object[] arguments = new object[] { value };
    //        proxy.Invoke(target, currentMethod.GetBaseDefinition(), arguments);

    //        value = (string)arguments[0];
    //    }

    //    public override object ReturnsClassType()
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        return proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
    //    }

    //    public override int ReturnsValueType()
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        return (int)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[0]);
    //    }

    //    public override int TwentyParameters(int p0,
    //                                         int p1,
    //                                         int p2,
    //                                         int p3,
    //                                         int p4,
    //                                         int p5,
    //                                         int p6,
    //                                         int p7,
    //                                         int p8,
    //                                         int p9,
    //                                         int p10,
    //                                         int p11,
    //                                         int p12,
    //                                         int p13,
    //                                         int p14,
    //                                         int p15,
    //                                         int p16,
    //                                         int p17,
    //                                         int p18,
    //                                         int p19)
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        return (int)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19 });
    //    }

    //    public override string TwoParameters(int x,
    //                                         string y)
    //    {
    //        MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
    //        return (string)proxy.Invoke(target, currentMethod.GetBaseDefinition(), new object[] { x, y });
    //    }
    //}

    public struct ComplexValueType
    {
        public byte Byte;
        public char Char;
        public decimal Decimal;
        public double Double;
        public float Float;
        public int Int;
        public long Long;
        public short Short;
        public string String;
        public uint UInt;
        public ulong ULong;
        public ushort UShort;
    }
}