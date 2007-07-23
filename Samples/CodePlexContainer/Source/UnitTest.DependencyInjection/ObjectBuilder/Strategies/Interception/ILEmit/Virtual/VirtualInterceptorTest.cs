using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualInterceptorTest
    {
        static T WrapAndCreateType<T>(IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers,
                                      params object[] ctorArgs)
        {
            Type wrappedType = VirtualInterceptor.WrapClass(typeof(T));
            ILEmitProxy proxy = new ILEmitProxy(handlers);
            List<object> wrappedCtorArgs = new List<object>();
            wrappedCtorArgs.Add(proxy);
            wrappedCtorArgs.AddRange(ctorArgs);
            return (T)Activator.CreateInstance(wrappedType, wrappedCtorArgs.ToArray());
        }

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

        [TestFixture]
        public class Errors
        {
            [Test]
            public void CannotInterceptSealedClass()
            {
                Assert.Throws<TypeLoadException>(
                    delegate
                    {
                        VirtualInterceptor.WrapClass(typeof(SealedClass));
                    });
            }

            [Test]
            public void CannotInterceptNonPublicClass()
            {
                Assert.Throws<TypeLoadException>(
                    delegate
                    {
                        VirtualInterceptor.WrapClass(typeof(PrivateClass));
                    });
            }

            public sealed class SealedClass {}

            class PrivateClass {}
        }

        [TestFixture]
        public class GenericClasses
        {
            [Test]
            public void NonGenericMethod()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(GenericSpy<>).GetMethod("MethodWhichTakesGenericData");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                GenericSpy<int> result = WrapAndCreateType<GenericSpy<int>>(dictionary);
                result.MethodWhichTakesGenericData(24);

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data 24", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void GenericMethod()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(GenericSpy<>).GetMethod("GenericMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                GenericSpy<int> result = WrapAndCreateType<GenericSpy<int>>(dictionary);
                result.GenericMethod(46, "2");

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data 46 and 2", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void ReturnsDataOfGenericType()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(GenericSpy<>).GetMethod("GenericReturn");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                GenericSpy<int> result = WrapAndCreateType<GenericSpy<int>>(dictionary);
                int value = result.GenericReturn(256.9);

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data 256.9", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
                Assert.Equal(default(int), value);
            }

            [Test]
            public void WhereClauseOnClass()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(GenericSpyWithWhereClause<>).GetMethod("Method");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                GenericSpyWithWhereClause<Foo> result = WrapAndCreateType<GenericSpyWithWhereClause<Foo>>(dictionary);
                result.Method(new Foo());

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data " + typeof(Foo).FullName, Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            public class GenericSpy<T>
            {
                public virtual void GenericMethod<T1>(T data,
                                                      T1 data1)
                {
                    Recorder.Records.Add("In method with data " + data + " and " + data1);
                }

                public virtual T GenericReturn<T1>(T1 data)
                {
                    Recorder.Records.Add("In method with data " + data);
                    return default(T);
                }

                public virtual void MethodWhichTakesGenericData(T data)
                {
                    Recorder.Records.Add("In method with data " + data);
                }
            }

            public class GenericSpyWithWhereClause<T>
                where T : class, IFoo
            {
                public virtual void Method(T data)
                {
                    Recorder.Records.Add("In method with data " + data);
                }
            }

            public interface IFoo {}

            public class Foo : IFoo {}
        }

        [TestFixture]
        public class GenericMethods
        {
            [Test]
            public void GenericMethod()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(NonGenericSpy).GetMethod("GenericMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                NonGenericSpy result = WrapAndCreateType<NonGenericSpy>(dictionary);
                result.GenericMethod(21);

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data 21", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void ReturnsDataOfGenericType()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(NonGenericSpy).GetMethod("GenericReturn");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                NonGenericSpy result = WrapAndCreateType<NonGenericSpy>(dictionary);
                int value = result.GenericReturn<int, double>(256.9);

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data 256.9", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
                Assert.Equal(default(int), value);
            }

            [Test]
            public void WhereClauseOnMethod()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(NonGenericSpy).GetMethod("WhereMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                NonGenericSpy result = WrapAndCreateType<NonGenericSpy>(dictionary);
                result.WhereMethod(new Foo());

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data " + typeof(Foo).FullName, Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            public class NonGenericSpy
            {
                public virtual void GenericMethod<T>(T data)
                {
                    Recorder.Records.Add("In method with data " + data);
                }

                public virtual T GenericReturn<T, T1>(T1 data)
                {
                    Recorder.Records.Add("In method with data " + data);
                    return default(T);
                }

                public virtual void WhereMethod<T>(T data)
                    where T : class, IFoo
                {
                    Recorder.Records.Add("In method with data " + data);
                }
            }

            public interface IFoo {}

            public class Foo : IFoo {}
        }

        [TestFixture]
        public class InParameters
        {
            [Test]
            public void OneParameter()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyIn).GetMethod("OneParameter");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyIn result = WrapAndCreateType<SpyIn>(dictionary);
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
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyIn).GetMethod("TwoParameters");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyIn result = WrapAndCreateType<SpyIn>(dictionary);
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
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyIn).GetMethod("TwentyParameters");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyIn result = WrapAndCreateType<SpyIn>(dictionary);
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
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyOut).GetMethod("OutReferenceType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = WrapAndCreateType<SpyOut>(dictionary);
                string outReference;
                result.OutReferenceType(out outReference);

                Assert.Equal("Hello, world!", outReference);
            }

            [Test]
            public void OutInt16()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyOut).GetMethod("OutInt16");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = WrapAndCreateType<SpyOut>(dictionary);
                short outValue;
                result.OutInt16(out outValue);

                Assert.Equal(short.MaxValue, outValue);
            }

            [Test]
            public void OutInt32()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyOut).GetMethod("OutInt32");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = WrapAndCreateType<SpyOut>(dictionary);
                int outValue;
                result.OutInt32(out outValue);

                Assert.Equal(int.MaxValue, outValue);
            }

            [Test]
            public void OutInt64()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyOut).GetMethod("OutInt64");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = WrapAndCreateType<SpyOut>(dictionary);
                long outValue;
                result.OutInt64(out outValue);

                Assert.Equal(long.MaxValue, outValue);
            }

            [Test]
            public void OutDouble()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyOut).GetMethod("OutDouble");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = WrapAndCreateType<SpyOut>(dictionary);
                double outValue;
                result.OutDouble(out outValue);

                Assert.Equal(double.MaxValue, outValue);
            }

            [Test]
            public void OutComplexValueType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyOut).GetMethod("OutComplexValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyOut result = WrapAndCreateType<SpyOut>(dictionary);
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
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyRef).GetMethod("RefClassParameter");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyRef result = WrapAndCreateType<SpyRef>(dictionary);
                string refValue = "Hello, ";
                result.RefClassParameter(ref refValue);

                Assert.Equal("Hello, world!", refValue);
            }

            [Test]
            public void RefValueType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyRef).GetMethod("RefValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyRef result = WrapAndCreateType<SpyRef>(dictionary);
                int refValue = 21;
                result.RefValueType(ref refValue);

                Assert.Equal(42, refValue);
            }

            public class SpyRef
            {
                public virtual void RefClassParameter(ref string value)
                {
                    value += "world!";
                }

                public virtual void RefValueType(ref int value)
                {
                    value *= 2;
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
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyReturn).GetMethod("NoReturnValue");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = WrapAndCreateType<SpyReturn>(dictionary, 42);
                result.NoReturnValue();

                Assert.Equal(42, result.ConstructorValue);
                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void ReturnsClassType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyReturn).GetMethod("ReturnsClassType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = WrapAndCreateType<SpyReturn>(dictionary, 42);
                object retValue = result.ReturnsClassType();

                Assert.Same(SpyReturn.ObjectReturn, retValue);
            }

            [Test]
            public void ReturnsValueType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyReturn).GetMethod("ReturnsValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = WrapAndCreateType<SpyReturn>(dictionary, 42);
                int retValue = result.ReturnsValueType();

                Assert.Equal(SpyReturn.ValueReturn, retValue);
            }

            [Test]
            public void Exception()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(SpyReturn).GetMethod("Exception");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                SpyReturn result = WrapAndCreateType<SpyReturn>(dictionary, 42);
                Assert.Throws<ArgumentException>(delegate
                                                 {
                                                     result.Exception();
                                                 });
            }

            public class SpyReturn
            {
                public const int ValueReturn = 42;
                public readonly int ConstructorValue;
                public static object ObjectReturn = new object();

                public SpyReturn(int x)
                {
                    ConstructorValue = x;
                }

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
}