using System;

namespace CodePlex.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true, Inherited = true)]
    public class EventSourceAttribute : Attribute
    {
        readonly string name;

        public EventSourceAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}