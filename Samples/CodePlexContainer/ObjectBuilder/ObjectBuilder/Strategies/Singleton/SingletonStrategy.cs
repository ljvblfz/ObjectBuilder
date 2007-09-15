namespace ObjectBuilder
{
    public class SingletonStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            if (context.Locator == null || context.Lifetime == null)
                return base.BuildUp(context, buildKey, existing);

            if (context.Locator.Contains(buildKey))
                return context.Locator.Get(buildKey);

            existing = base.BuildUp(context, buildKey, existing);

            ISingletonPolicy singletonPolicy = context.Policies.Get<ISingletonPolicy>(buildKey);

            if (singletonPolicy != null && singletonPolicy.IsSingleton)
            {
                context.Locator.Add(buildKey, existing);
                context.Lifetime.Add(existing);
            }

            return existing;
        }
    }
}