using System;

namespace ObjectBuilder
{
    public struct NamedTypeBuildKey : ITypeBasedBuildKey
    {
        readonly string name;
        readonly Type type;

        public NamedTypeBuildKey(Type type,
                                 string name)
        {
            this.type = type;
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return type; }
        }
    }
}