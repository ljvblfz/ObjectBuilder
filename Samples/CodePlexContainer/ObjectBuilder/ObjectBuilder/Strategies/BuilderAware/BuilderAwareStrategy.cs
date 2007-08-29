namespace ObjectBuilder
{
    public class BuilderAwareStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            IBuilderAware awareObject = existing as IBuilderAware;

            if (awareObject != null)
                awareObject.OnBuiltUp(buildKey);

            return base.BuildUp(context, buildKey, existing);
        }

        public override object TearDown(IBuilderContext context,
                                        object item)
        {
            IBuilderAware awareObject = item as IBuilderAware;

            if (awareObject != null)
                awareObject.OnTearingDown();

            return base.TearDown(context, item);
        }
    }
}