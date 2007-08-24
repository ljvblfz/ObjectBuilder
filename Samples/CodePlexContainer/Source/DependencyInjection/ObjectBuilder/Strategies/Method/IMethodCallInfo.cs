namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodCallInfo
    {
        void Execute(IBuilderContext context,
                     object instance,
                     object buildKey);
    }
}