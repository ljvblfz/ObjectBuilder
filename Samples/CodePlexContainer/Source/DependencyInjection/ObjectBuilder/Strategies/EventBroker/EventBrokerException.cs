using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class EventBrokerException : Exception
    {
        // Fields

        List<Exception> exceptions;

        // Lifetime

        public EventBrokerException()
            : base("One or more exceptions were thrown by event broker sinks") {}

        public EventBrokerException(IEnumerable<Exception> exceptions)
            : this()
        {
            this.exceptions = new List<Exception>(exceptions);
        }

        // Properties

        public ReadOnlyCollection<Exception> Exceptions
        {
            get { return exceptions.AsReadOnly(); }
        }
    }
}