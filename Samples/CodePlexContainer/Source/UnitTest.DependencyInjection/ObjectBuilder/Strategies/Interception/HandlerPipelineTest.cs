using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class HandlerPipelineTest
    {
        [Test]
        public void NullHandlerLists()
        {
            Assert.Throws<ArgumentNullException>(delegate
                                                 {
                                                     new HandlerPipeline((IEnumerable<IInterceptionHandler>)null);
                                                 });

            Assert.Throws<ArgumentNullException>(delegate
                                                 {
                                                     new HandlerPipeline((IInterceptionHandler[])null);
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
            RecordingHandler handler = new RecordingHandler();
            StubMethodInvocation invocation = new StubMethodInvocation();
            HandlerPipeline pipeline = new HandlerPipeline(handler);

            pipeline.Invoke(invocation, delegate
                                        {
                                            Recorder.Records.Add("method");
                                            return null;
                                        });

            Assert.Equal(3, Recorder.Records.Count);
            Assert.Equal("Before Method", Recorder.Records[0]);
            Assert.Equal("method", Recorder.Records[1]);
            Assert.Equal("After Method", Recorder.Records[2]);
        }

        [Test]
        public void TwoHandlers()
        {
            Recorder.Records.Clear();
            RecordingHandler handler1 = new RecordingHandler("1");
            RecordingHandler handler2 = new RecordingHandler("2");
            StubMethodInvocation invocation = new StubMethodInvocation();
            HandlerPipeline pipeline = new HandlerPipeline(handler1, handler2);

            pipeline.Invoke(invocation, delegate
                                        {
                                            Recorder.Records.Add("method");
                                            return null;
                                        });

            Assert.Equal(5, Recorder.Records.Count);
            Assert.Equal("Before Method (1)", Recorder.Records[0]);
            Assert.Equal("Before Method (2)", Recorder.Records[1]);
            Assert.Equal("method", Recorder.Records[2]);
            Assert.Equal("After Method (2)", Recorder.Records[3]);
            Assert.Equal("After Method (1)", Recorder.Records[4]);
        }
    }
}