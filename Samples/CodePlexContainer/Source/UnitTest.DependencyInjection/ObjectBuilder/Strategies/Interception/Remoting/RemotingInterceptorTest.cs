using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class RemotingInterceptorTest
    {
        [Test]
        public void CanInterceptClassDerivedFromMBRO()
        {
            Recorder.Records.Clear();
            SpyMBROClass rawObject = new SpyMBROClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = rawObject.GetType().GetMethod("InterceptedMethod");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            SpyMBROClass result = RemotingInterceptor.Wrap(rawObject, dictionary);
            result.InterceptedMethod();

            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void CanInterceptInterface()
        {
            Recorder.Records.Clear();
            SpyInterfaceClass rawObject = new SpyInterfaceClass();
            RecordingHandler handler = new RecordingHandler();
            MethodBase method = typeof(ISpyInterface).GetMethod("InterceptedMethod");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            ISpyInterface result = RemotingInterceptor.Wrap<ISpyInterface>(rawObject, dictionary);
            result.InterceptedMethod();

            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void PassesParameters()
        {
            Recorder.Records.Clear();
            SpyWithParameters rawObject = new SpyWithParameters();
            MethodBase method = typeof(SpyWithParameters).GetMethod("InterceptedMethod");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            dictionary.Add(method, handlers);
            int i = 9;
            string s;

            SpyWithParameters wrapped = RemotingInterceptor.Wrap(rawObject, dictionary);
            int result = wrapped.InterceptedMethod(4.2, ref i, out s);

            Assert.Equal(42, result);
            Assert.Equal(18, i);
            Assert.Equal("MyString", s);
            Assert.Equal("d = 4.2", Recorder.Records[0]);
        }

        [Test]
        public void InterceptorCanInfluenceParameters()
        {
            Recorder.Records.Clear();
            SpyWithParameters rawObject = new SpyWithParameters();
            MethodBase method = typeof(SpyWithParameters).GetMethod("InterceptedMethod");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(new SpyWithParametersHandler());
            dictionary.Add(method, handlers);
            int i = 9;
            string s;

            SpyWithParameters wrapped = RemotingInterceptor.Wrap(rawObject, dictionary);
            int result = wrapped.InterceptedMethod(4.2, ref i, out s);

            Assert.Equal(46 & 2, result);
            Assert.Equal(16, i);
            Assert.Equal("ANewString", s);
            Assert.Equal("d = 6.4", Recorder.Records[0]);
        }

        [Test]
        public void Exceptions()
        {
            Recorder.Records.Clear();
            SpyException rawObject = new SpyException();
            MethodBase method = typeof(SpyException).GetMethod("InterceptedMethod");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(new RecordingHandler());
            dictionary.Add(method, handlers);

            SpyException wrapped = RemotingInterceptor.Wrap(rawObject, dictionary);

            Assert.Throws<NotImplementedException>(delegate
                                                   {
                                                       wrapped.InterceptedMethod();
                                                   });

            Assert.Equal(2, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("After Method", Recorder.Records[1]);
        }

        [Test]
        public void ShortCircuit()
        {
            Recorder.Records.Clear();
            SpyException rawObject = new SpyException();
            MethodBase method = typeof(SpyException).GetMethod("InterceptedMethod");
            Dictionary<MethodBase, List<IInterceptionHandler>> dictionary = new Dictionary<MethodBase, List<IInterceptionHandler>>();
            List<IInterceptionHandler> handlers = new List<IInterceptionHandler>();
            handlers.Add(new ShortCircuitHandler());
            dictionary.Add(method, handlers);

            SpyException wrapped = RemotingInterceptor.Wrap(rawObject, dictionary);
            wrapped.InterceptedMethod(); // Does not throw because it was short circuited
        }

        // Helpers

        internal class SpyMBROClass : MarshalByRefObject
        {
            public void InterceptedMethod()
            {
                Recorder.Records.Add("In Method");
            }
        }

        internal interface ISpyInterface
        {
            void InterceptedMethod();
        }

        internal class SpyInterfaceClass : ISpyInterface
        {
            public void InterceptedMethod()
            {
                Recorder.Records.Add("In Method");
            }
        }

        internal class SpyWithParameters : MarshalByRefObject
        {
            public int InterceptedMethod(double d,
                                         ref int i,
                                         out string s)
            {
                Recorder.Records.Add("d = " + d);
                i *= 2;
                s = "MyString";
                return 42;
            }
        }

        internal class SpyWithParametersHandler : IInterceptionHandler
        {
            public IMethodReturn Invoke(IMethodInvocation call,
                                        GetNextHandlerDelegate getNext)
            {
                call.Inputs["d"] = 6.4;
                call.Inputs["i"] = 8;
                IMethodReturn result = getNext().Invoke(call, getNext);
                result.Outputs["s"] = "ANewString";
                result.ReturnValue = 46 & 2;
                return result;
            }
        }

        internal class SpyException : MarshalByRefObject
        {
            public void InterceptedMethod()
            {
                throw new NotImplementedException();
            }
        }

        internal class ShortCircuitHandler : IInterceptionHandler
        {
            public IMethodReturn Invoke(IMethodInvocation call,
                                        GetNextHandlerDelegate getNext)
            {
                return new StubMethodReturn();
            }
        }
    }
}