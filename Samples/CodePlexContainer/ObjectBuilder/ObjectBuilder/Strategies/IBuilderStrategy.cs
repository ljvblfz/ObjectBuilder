namespace ObjectBuilder
{
    public interface IBuilderStrategy
    {
        object BuildUp(IBuilderContext context,
                       object buildKey,
                       object existing);

        object TearDown(IBuilderContext context,
                        object item);
    }
}