using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class TypeMappingPolicy : ITypeMappingPolicy
    {
        readonly DependencyResolutionLocatorKey pair;

        public TypeMappingPolicy(Type type,
                                 string id)
        {
            pair = new DependencyResolutionLocatorKey(type, id);
        }

        public DependencyResolutionLocatorKey Map(DependencyResolutionLocatorKey incomingTypeIDPair)
        {
            return pair;
        }
    }
}