namespace ObjectBuilder
{
    public interface ISingletonPolicy : IBuilderPolicy
    {
        bool IsSingleton { get; }
    }
}