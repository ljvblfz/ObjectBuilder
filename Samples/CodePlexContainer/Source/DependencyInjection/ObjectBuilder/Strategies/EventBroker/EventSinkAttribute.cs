using System;

namespace CodePlex.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class EventSinkAttribute : Attribute
    {
        // Fields

        readonly string name;

        // Lifetime

        public EventSinkAttribute(string name)
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