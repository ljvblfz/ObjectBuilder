using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class TypeMappingPolicy : ITypeMappingPolicy
    {
        // Fields

        DependencyResolutionLocatorKey pair;

        // Lifetime

        public TypeMappingPolicy(Type type,
                                 string id)
        {
            pair = new DependencyResolutionLocatorKey(type, id);
        }

        // Methods

        public DependencyResolutionLocatorKey Map(DependencyResolutionLocatorKey incomingTypeIDPair)
        {
            return pair;
        }
    }
}