using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ObjectBuilder
{
    public class EventBrokerException : Exception
    {
        readonly List<Exception> exceptions;

        public EventBrokerException()
            : base("One or more exceptions were thrown by event broker sinks") {}

        public EventBrokerException(IEnumerable<Exception> exceptions)
            : this()
        {
            this.exceptions = new List<Exception>(exceptions);
        }

        public ReadOnlyCollection<Exception> Exceptions
        {
            get { return exceptions.AsReadOnly(); }
        }
    }
}