namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilderAware
    {
        // Methods

        void OnBuiltUp(string id);
        void OnTearingDown();
    }
}