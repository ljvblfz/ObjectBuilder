using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public sealed class DependencyResolutionLocatorKey
    {
        readonly string id;
        readonly Type type;

        public DependencyResolutionLocatorKey()
            : this(null, null) {}

        public DependencyResolutionLocatorKey(Type type,
                                              string id)
        {
            this.type = type;
            this.id = id;
        }

        public string ID
        {
            get { return id; }
        }

        public Type Type
        {
            get { return type; }
        }

        public override bool Equals(object obj)
        {
            DependencyResolutionLocatorKey other = obj as DependencyResolutionLocatorKey;

            if (other == null)
                return false;

            return (Equals(type, other.type) && Equals(id, other.id));
        }

        public override int GetHashCode()
        {
            int hashForType = type == null ? 0 : type.GetHashCode();
            int hashForID = id == null ? 0 : id.GetHashCode();
            return hashForType ^ hashForID;
        }
    }
}