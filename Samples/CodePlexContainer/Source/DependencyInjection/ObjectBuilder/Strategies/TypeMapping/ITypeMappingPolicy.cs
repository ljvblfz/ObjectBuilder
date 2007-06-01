namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface ITypeMappingPolicy : IBuilderPolicy
    {
        // Methods

        DependencyResolutionLocatorKey Map(DependencyResolutionLocatorKey incomingTypeIDPair);
    }
}