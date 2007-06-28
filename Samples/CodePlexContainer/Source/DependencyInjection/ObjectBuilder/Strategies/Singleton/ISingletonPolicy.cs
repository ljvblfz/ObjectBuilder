namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface ISingletonPolicy : IBuilderPolicy
    {
        bool IsSingleton { get; }
    }
}