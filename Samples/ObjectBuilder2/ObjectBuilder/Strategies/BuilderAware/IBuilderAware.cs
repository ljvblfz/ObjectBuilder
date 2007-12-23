namespace ObjectBuilder
{
    public interface IBuilderAware
    {
        void OnBuiltUp(object buildKey);
        void OnTearingDown();
    }
}