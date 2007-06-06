using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection
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
            Dictionary<MethodBase, List<ICallHandler>> dictionary = new Dictionary<MethodBase, List<ICallHandler>>();
            List<ICallHandler> handlers = new List<ICallHandler>();
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
            Dictionary<MethodBase, List<ICallHandler>> dictionary = new Dictionary<MethodBase, List<ICallHandler>>();
            List<ICallHandler> handlers = new List<ICallHandler>();
            handlers.Add(handler);
            dictionary.Add(method, handlers);

            ISpyInterface result = RemotingInterceptor.Wrap<ISpyInterface>(rawObject, dictionary);
            result.InterceptedMethod();

            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("In Method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        // Helpers

        static class Recorder
        {
            public static List<string> Records = new List<string>();
        }

        class RecordingHandler : ICallHandler
        {
            public IMethodReturn Invoke(IMethodInvocation call,
                                        GetNextHandlerDelegate getNext)
            {
                Recorder.Records.Add("Before Method");
                IMethodReturn result = getNext().Invoke(call, getNext);
                Recorder.Records.Add("After Method");
                return result;
            }
        }

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
    }
}