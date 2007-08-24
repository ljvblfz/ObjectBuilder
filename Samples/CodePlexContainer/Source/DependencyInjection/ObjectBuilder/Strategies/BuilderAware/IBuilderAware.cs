namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilderAware
    {
        void OnBuiltUp(object buildKey);
        void OnTearingDown();
    }
}