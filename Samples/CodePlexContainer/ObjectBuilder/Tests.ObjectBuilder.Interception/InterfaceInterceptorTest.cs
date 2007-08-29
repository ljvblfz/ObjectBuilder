using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace ObjectBuilder
{
    public class InterfaceInterceptorTest
    {
        static TInterface WrapAndCreateType<TInterface, TConcrete>(IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
            where TConcrete : TInterface
        {
            Type wrappedType = InterfaceInterceptor.WrapInterface(typeof(TInterface));
            ILEmitProxy proxy = new ILEmitProxy(handlers);
            object target = Activator.CreateInstance(typeof(TConcrete));
            List<object> wrappedCtorArgs = new List<object>();
            wrappedCtorArgs.Add(proxy);
            wrappedCtorArgs.Add(target);
            return (TInterface)Activator.CreateInstance(wrappedType, wrappedCtorArgs.ToArray());
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
        public class FindMethod
        {
            [Test]
            public void MethodNotFound()
            {
                MethodInfo result = InterfaceInterceptor.FindMethod("ThisMethodDoesNotExist", new Type[0], typeof(Object).GetMethods());

                Assert.Null(result);
            }

            [Test]
            public void ParameterTypesNotMatching()
            {
                MethodInfo result = InterfaceInterceptor.FindMethod("ToString", new Type[] { typeof(int) }, typeof(Object).GetMethods());

                Assert.Null(result);
            }

            [Test]
            public void NonGenericClass()
            {
                MethodInfo result = InterfaceInterceptor.FindMethod("ToString", new Type[0], typeof(Object).GetMethods());

                Assert.Same(typeof(Object).GetMethod("ToString"), result);
            }

            [Test]
            public void GenericClassNonGenericMethod()
            {
                MethodInfo result = InterfaceInterceptor.FindMethod("RemoveAt", new Type[] { typeof(int) }, typeof(IList<>).GetMethods());

                Assert.Same(typeof(IList<>).GetMethod("RemoveAt"), result);
            }

            [Test]
            public void GenericClassGenericMethod()
            {
                MethodInfo result = InterfaceInterceptor.FindMethod("Insert", new object[] { typeof(int), "T" }, typeof(IList<>).GetMethods());

                Assert.Same(typeof(IList<>).GetMethod("Insert"), result);
            }

            [Test]
            public void GenericClassGenericMethodWithExtraGenerics()
            {
                MethodInfo result = InterfaceInterceptor.FindMethod("Bar", new object[] { "T", "T2" }, typeof(IFoo<>).GetMethods());

                Assert.Same(typeof(IFoo<>).GetMethod("Bar"), result);
            }

            [Test]
            public void Overloads()
            {
                MethodInfo result1 = InterfaceInterceptor.FindMethod("Overload", new Type[0], typeof(IFoo<>).GetMethods());
                MethodInfo result2 = InterfaceInterceptor.FindMethod("Overload", new Type[] { typeof(int) }, typeof(IFoo<>).GetMethods());
                MethodInfo result3 = InterfaceInterceptor.FindMethod("Overload", new Type[] { typeof(double) }, typeof(IFoo<>).GetMethods());

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.Null(result3);
                Assert.NotSame(result1, result2);
            }

            public interface IFoo<T>
            {
                void Bar<T2>(T data,
                             T2 data2);

                void Overload();
                void Overload(int x);
            }

            public class Foo<T> : IFoo<T>
            {
                public void Bar<T2>(T data,
                                    T2 data2) {}

                public void Baz() {}

                public void Overload() {}

                public void Overload(int x) {}
            }
        }

        [TestFixture]
        public class GenericClasses
        {
            [Test]
            public void NonGenericMethod()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(IGenericSpy<>).GetMethod("MethodWhichTakesGenericData");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                IGenericSpy<int> result = WrapAndCreateType<IGenericSpy<int>, GenericSpy<int>>(dictionary);
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
                MethodBase method = typeof(IGenericSpy<>).GetMethod("GenericMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                IGenericSpy<int> result = WrapAndCreateType<IGenericSpy<int>, GenericSpy<int>>(dictionary);
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
                MethodBase method = typeof(IGenericSpy<>).GetMethod("GenericReturn");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                IGenericSpy<int> result = WrapAndCreateType<IGenericSpy<int>, GenericSpy<int>>(dictionary);
                int value = result.GenericReturn(256.9);

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data 256.9", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
                Assert.Equal(default(int), value);
            }

            [Test]
            public void WhereClauseOnInterface()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(IGenericSpyWithWhereClause<>).GetMethod("Method");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                IGenericSpyWithWhereClause<Foo> result = WrapAndCreateType<IGenericSpyWithWhereClause<Foo>, GenericSpyWithWhereClause<Foo>>(dictionary);
                result.Method(new Foo());

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data " + typeof(Foo).FullName, Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            public interface IGenericSpy<T>
            {
                void GenericMethod<T1>(T data,
                                       T1 data1);

                T GenericReturn<T1>(T1 data);
                void MethodWhichTakesGenericData(T data);
            }

            public class GenericSpy<T> : IGenericSpy<T>
            {
                public void GenericMethod<T1>(T data,
                                              T1 data1)
                {
                    Recorder.Records.Add("In method with data " + data + " and " + data1);
                }

                public T GenericReturn<T1>(T1 data)
                {
                    Recorder.Records.Add("In method with data " + data);
                    return default(T);
                }

                public void MethodWhichTakesGenericData(T data)
                {
                    Recorder.Records.Add("In method with data " + data);
                }
            }

            public interface IGenericSpyWithWhereClause<T>
                where T : class, IFoo
            {
                void Method(T data);
            }

            public class GenericSpyWithWhereClause<T> : IGenericSpyWithWhereClause<T>
                where T : class, IFoo
            {
                public void Method(T data)
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
                MethodBase method = typeof(INonGenericSpy).GetMethod("GenericMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                INonGenericSpy result = WrapAndCreateType<INonGenericSpy, NonGenericSpy>(dictionary);
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
                MethodBase method = typeof(INonGenericSpy).GetMethod("GenericReturn");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                INonGenericSpy result = WrapAndCreateType<INonGenericSpy, NonGenericSpy>(dictionary);
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
                MethodBase method = typeof(INonGenericSpy).GetMethod("WhereMethod");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                INonGenericSpy result = WrapAndCreateType<INonGenericSpy, NonGenericSpy>(dictionary);
                result.WhereMethod(new Foo());

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In method with data " + typeof(Foo).FullName, Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            public interface INonGenericSpy
            {
                void GenericMethod<T>(T data);
                T GenericReturn<T, T1>(T1 data);

                void WhereMethod<T>(T data)
                    where T : class, IFoo;
            }

            public class NonGenericSpy : INonGenericSpy
            {
                public void GenericMethod<T>(T data)
                {
                    Recorder.Records.Add("In method with data " + data);
                }

                public T GenericReturn<T, T1>(T1 data)
                {
                    Recorder.Records.Add("In method with data " + data);
                    return default(T);
                }

                public void WhereMethod<T>(T data)
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
            public void InReferenceParameter()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyIn).GetMethod("InReferenceParameter");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyIn result = WrapAndCreateType<ISpyIn, SpyIn>(dictionary);
                result.InReferenceParameter("Hello");

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method: Hello", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void InValueParameter()
            {
                Recorder.Records.Clear();
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyIn).GetMethod("InValueParameter");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyIn result = WrapAndCreateType<ISpyIn, SpyIn>(dictionary);
                result.InValueParameter(42);

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method: 42", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            public interface ISpyIn
            {
                void InReferenceParameter(string s);
                void InValueParameter(int x);
            }

            sealed class SpyIn : ISpyIn
            {
                void ISpyIn.InReferenceParameter(string s)
                {
                    Recorder.Records.Add("In Method: " + s);
                }

                public void InValueParameter(int x)
                {
                    Recorder.Records.Add("In Method: " + x);
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
                MethodBase method = typeof(ISpyOut).GetMethod("OutReferenceType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyOut result = WrapAndCreateType<ISpyOut, SpyOut>(dictionary);
                string outReference;
                result.OutReferenceType(out outReference);

                Assert.Equal("Hello, world!", outReference);
            }

            [Test]
            public void OutInt16()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyOut).GetMethod("OutInt16");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyOut result = WrapAndCreateType<ISpyOut, SpyOut>(dictionary);
                short outValue;
                result.OutInt16(out outValue);

                Assert.Equal(short.MaxValue, outValue);
            }

            [Test]
            public void OutInt32()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyOut).GetMethod("OutInt32");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyOut result = WrapAndCreateType<ISpyOut, SpyOut>(dictionary);
                int outValue;
                result.OutInt32(out outValue);

                Assert.Equal(int.MaxValue, outValue);
            }

            [Test]
            public void OutInt64()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyOut).GetMethod("OutInt64");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyOut result = WrapAndCreateType<ISpyOut, SpyOut>(dictionary);
                long outValue;
                result.OutInt64(out outValue);

                Assert.Equal(long.MaxValue, outValue);
            }

            [Test]
            public void OutDouble()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyOut).GetMethod("OutDouble");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyOut result = WrapAndCreateType<ISpyOut, SpyOut>(dictionary);
                double outValue;
                result.OutDouble(out outValue);

                Assert.Equal(double.MaxValue, outValue);
            }

            [Test]
            public void OutComplexValueType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyOut).GetMethod("OutComplexValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyOut result = WrapAndCreateType<ISpyOut, SpyOut>(dictionary);
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

            public interface ISpyOut
            {
                void OutChar(out char outValue);
                void OutComplexValueType(out ComplexValueType outValueType);
                void OutDouble(out double outValue);
                void OutInt16(out short outValue);
                void OutInt32(out int outValue);
                void OutInt64(out long outValue);
                void OutReferenceType(out string outReference);
            }

            sealed class SpyOut : ISpyOut
            {
                public void OutChar(out char outValue)
                {
                    outValue = 'a';
                }

                public void OutComplexValueType(out ComplexValueType outValueType)
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

                public void OutDouble(out double outValue)
                {
                    outValue = double.MaxValue;
                }

                public void OutInt16(out short outValue)
                {
                    outValue = short.MaxValue;
                }

                public void OutInt32(out int outValue)
                {
                    outValue = int.MaxValue;
                }

                public void OutInt64(out long outValue)
                {
                    outValue = long.MaxValue;
                }

                public void OutReferenceType(out string outReference)
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
                MethodBase method = typeof(ISpyRef).GetMethod("RefClassParameter");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyRef result = WrapAndCreateType<ISpyRef, SpyRef>(dictionary);
                string refValue = "Hello, ";
                result.RefClassParameter(ref refValue);

                Assert.Equal("Hello, world!", refValue);
            }

            [Test]
            public void RefValueType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyRef).GetMethod("RefValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyRef result = WrapAndCreateType<ISpyRef, SpyRef>(dictionary);
                int refValue = 21;
                result.RefValueType(ref refValue);

                Assert.Equal(42, refValue);
            }

            public interface ISpyRef
            {
                void RefClassParameter(ref string value);
                void RefValueType(ref int value);
            }

            sealed class SpyRef : ISpyRef
            {
                public void RefClassParameter(ref string value)
                {
                    value += "world!";
                }

                public void RefValueType(ref int value)
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
                MethodBase method = typeof(ISpyReturn).GetMethod("NoReturnValue");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyReturn result = WrapAndCreateType<ISpyReturn, SpyReturn>(dictionary);
                result.NoReturnValue();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void ReturnsClassType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyReturn).GetMethod("ReturnsClassType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyReturn result = WrapAndCreateType<ISpyReturn, SpyReturn>(dictionary);
                object retValue = result.ReturnsClassType();

                Assert.Same(SpyReturn.ObjectReturn, retValue);
            }

            [Test]
            public void ReturnsValueType()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyReturn).GetMethod("ReturnsValueType");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyReturn result = WrapAndCreateType<ISpyReturn, SpyReturn>(dictionary);
                int retValue = result.ReturnsValueType();

                Assert.Equal(SpyReturn.ValueReturn, retValue);
            }

            [Test]
            public void Exception()
            {
                RecordingHandler handler = new RecordingHandler();
                MethodBase method = typeof(ISpyReturn).GetMethod("Exception");
                Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
                List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
                handlers.Add(handler);
                dictionary.Add(method, handlers);

                ISpyReturn result = WrapAndCreateType<ISpyReturn, SpyReturn>(dictionary);
                Assert.Throws<ArgumentException>(delegate
                                                 {
                                                     result.Exception();
                                                 });
            }

            public interface ISpyReturn
            {
                void Exception();
                void NoReturnValue();
                object ReturnsClassType();
                int ReturnsValueType();
            }

            sealed class SpyReturn : ISpyReturn
            {
                public const int ValueReturn = 42;
                public static object ObjectReturn = new object();

                public void Exception()
                {
                    throw new ArgumentException();
                }

                public void NoReturnValue()
                {
                    Recorder.Records.Add("In Method");
                }

                public object ReturnsClassType()
                {
                    return ObjectReturn;
                }

                public int ReturnsValueType()
                {
                    return ValueReturn;
                }
            }
        }
    }
}