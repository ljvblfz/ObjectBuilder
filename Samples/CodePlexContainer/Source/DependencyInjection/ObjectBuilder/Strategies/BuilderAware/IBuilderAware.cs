namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilderAware
    {
        void OnBuiltUp(string id);
        void OnTearingDown();
    }
}