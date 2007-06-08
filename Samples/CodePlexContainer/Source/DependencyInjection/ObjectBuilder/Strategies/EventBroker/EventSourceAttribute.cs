using System;

namespace CodePlex.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true, Inherited = true)]
    public class EventSourceAttribute : Attribute
    {
        // Fields

        string name;

        // Lifetime

        public EventSourceAttribute(string name)
        {
            this.name = name;
        }

        // Properties

        public string Name
        {
            get { return name; }
        }
    }
}