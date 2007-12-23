namespace ObjectBuilder
{
    public interface IBuildKeyMappingPolicy : IBuilderPolicy
    {
        object Map(object buildKey);
    }
}