namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface ISingletonPolicy : IBuilderPolicy
    {
        // Properties

        bool IsSingleton { get; }
    }
}