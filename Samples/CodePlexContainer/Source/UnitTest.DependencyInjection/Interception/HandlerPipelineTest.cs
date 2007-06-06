using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection
{
    [TestFixture]
    public class HandlerPipelineTest
    {
        [Test]
        public void NullHandlerLists()
        {
            Assert.Throws<ArgumentNullException>(delegate
                                                 {
                                                     new HandlerPipeline((IEnumerable<ICallHandler>)null);
                                                 });

            Assert.Throws<ArgumentNullException>(delegate
                                                 {
                                                     new HandlerPipeline((ICallHandler[])null);
                                                 });
        }

        [Test]
        public void NoHandlersCallsTarget()
        {
            bool called = false;
            StubMethodInvocation invocation = new StubMethodInvocation();
            StubMethodReturn returnValue = new StubMethodReturn();
            HandlerPipeline pipeline = new HandlerPipeline();

            IMethodReturn result = pipeline.Invoke(invocation, delegate(IMethodInvocation call,
                                                                        GetNextHandlerDelegate getNext)
                                                               {
                                                                   Assert.Same(call, invocation);
                                                                   Assert.Null(getNext);

                                                                   called = true;
                                                                   return returnValue;
                                                               });

            Assert.True(called);
            Assert.Same(returnValue, result);
        }

        [Test]
        public void OneHandler()
        {
            Recorder.Records.Clear();
            RecordingHandler handler = new RecordingHandler("handler");
            StubMethodInvocation invocation = new StubMethodInvocation();
            HandlerPipeline pipeline = new HandlerPipeline(handler);

            pipeline.Invoke(invocation, delegate
                                        {
                                            Recorder.Records.Add("method");
                                            return null;
                                        });

            Assert.Equal(2, Recorder.Records.Count);
            Assert.Equal("handler", Recorder.Records[0]);
            Assert.Equal("method", Recorder.Records[1]);
        }

        [Test]
        public void TwoHandlers()
        {
            Recorder.Records.Clear();
            RecordingHandler handler1 = new RecordingHandler("handler 1");
            RecordingHandler handler2 = new RecordingHandler("handler 2");
            StubMethodInvocation invocation = new StubMethodInvocation();
            HandlerPipeline pipeline = new HandlerPipeline(handler1, handler2);

            pipeline.Invoke(invocation, delegate
                                        {
                                            Recorder.Records.Add("method");
                                            return null;
                                        });

            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("handler 1", Recorder.Records[0]);
            Assert.Equal("handler 2", Recorder.Records[1]);
            Assert.Equal("method", Recorder.Records[2]);
        }

        // Helpers

        static class Recorder
        {
            public static List<string> Records = new List<string>();
        }

        class RecordingHandler : ICallHandler
        {
            // Fields

            string message;

            // Lifetime

            public RecordingHandler(string message)
            {
                this.message = message;
            }

            // Method

            public IMethodReturn Invoke(IMethodInvocation call,
                                        GetNextHandlerDelegate getNext)
            {
                Recorder.Records.Add(message);
                return getNext().Invoke(call, getNext);
            }
        }
    }
}