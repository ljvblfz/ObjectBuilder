namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuildKeyMappingPolicy : IBuilderPolicy
    {
        object Map(object buildKey);
    }
}