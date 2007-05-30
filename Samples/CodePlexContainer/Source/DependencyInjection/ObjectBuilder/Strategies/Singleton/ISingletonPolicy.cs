namespace CodePlex.DependencyInjection.ObjectBuilder
{
    /// <summary>
    /// Represents a policy for <see cref="SingletonStrategy"/>.
    /// </summary>
    public interface ISingletonPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Returns true if the object should be a singleton.
        /// </summary>
        bool IsSingleton { get; }
    }
}