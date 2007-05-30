namespace CodePlex.DependencyInjection.ObjectBuilder
{
    /// <summary>
    /// Represents a policy for <see cref="TypeMappingStrategy"/>.
    /// </summary>
    public interface ITypeMappingPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Maps one Type/ID pair to another.
        /// </summary>
        /// <param name="incomingTypeIDPair">The incoming Type/ID pair.</param>
        /// <returns>The new Type/ID pair.</returns>
        DependencyResolutionLocatorKey Map(DependencyResolutionLocatorKey incomingTypeIDPair);
    }
}