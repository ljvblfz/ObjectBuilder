namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface ITypeMappingPolicy : IBuilderPolicy
    {
        DependencyResolutionLocatorKey Map(DependencyResolutionLocatorKey incomingTypeIDPair);
    }
}