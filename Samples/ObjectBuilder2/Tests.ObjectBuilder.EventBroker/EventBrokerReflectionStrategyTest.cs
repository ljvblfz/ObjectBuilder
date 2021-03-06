using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace ObjectBuilder
{
    public class EventBrokerReflectionStrategyTest
    {
        [Fact]
        public void ClassWithSink()
        {
            MockBuilderContext context = new MockBuilderContext();
            EventBrokerReflectionStrategy strategy = new EventBrokerReflectionStrategy();

            strategy.BuildUp(context, typeof(Sink), null);

            IEventBrokerPolicy policy = context.Policies.Get<IEventBrokerPolicy>(typeof(Sink));
            Assert.NotNull(policy);
            Assert.Contains(new KeyValuePair<string, MethodInfo>("Bar", typeof(Sink).GetMethod("EventSinkMethod")), policy.Sinks);
        }

        [Fact]
        public void ClassWithSource()
        {
            MockBuilderContext context = new MockBuilderContext();
            EventBrokerReflectionStrategy strategy = new EventBrokerReflectionStrategy();

            strategy.BuildUp(context, typeof(Source), null);

            IEventBrokerPolicy policy = context.Policies.Get<IEventBrokerPolicy>(typeof(Source));
            Assert.NotNull(policy);
            Assert.Contains(new KeyValuePair<string, EventInfo>("Baz", typeof(Source).GetEvent("EventSource")), policy.Sources);
        }

        [Fact]
        public void IgnoresNonTypeBuildKeys()
        {
            MockBuilderContext context = new MockBuilderContext();
            EventBrokerReflectionStrategy strategy = new EventBrokerReflectionStrategy();

            strategy.BuildUp(context, "Foo", null);

            Assert.Null(context.Policies.Get<IEventBrokerPolicy>("Foo"));
        }

        [Fact]
        public void IgnoresTypesWithNoEventBrokerAttributes()
        {
            MockBuilderContext context = new MockBuilderContext();
            EventBrokerReflectionStrategy strategy = new EventBrokerReflectionStrategy();

            strategy.BuildUp(context, typeof(object), null);

            Assert.Null(context.Policies.Get<IEventBrokerPolicy>(typeof(object)));
        }

        class Sink
        {
#pragma warning disable 168
            [EventSink("Bar")]
            public void EventSinkMethod(object sender,
                                        EventArgs e) {}
#pragma warning restore 168
        }

        class Source
        {
#pragma warning disable 67
            [EventSource("Baz")]
            public event EventHandler EventSource;
#pragma warning restore 67
        }
    }
}