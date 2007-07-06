using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodInterfaceInterceptorTest
    {
        static TInterface WrapAndCreateType<TInterface, TConcrete>(IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
            where TConcrete : TInterface
        {
            Type wrappedType = VirtualMethodInterfaceInterceptor.WrapInterface(typeof(TInterface));
            VirtualMethodProxy proxy = new VirtualMethodProxy(handlers);
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
        public class Errors {}

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