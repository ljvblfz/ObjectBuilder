using System;

namespace ObjectBuilder
{
    public class InterceptionReflectionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            Type typeToBuild;

            if (TryGetTypeFromBuildKey(buildKey, out typeToBuild))
            {
                Type originalType;

                if (!TryGetTypeFromBuildKey(context.OriginalBuildKey, out originalType))
                    originalType = typeToBuild;

                IObjectFactory factory = context.Locator.Get<IObjectFactory>();

                if (factory != null)
                    InterceptionReflector.Reflect(originalType, typeToBuild, context.Policies, factory);
            }

            return base.BuildUp(context, buildKey, existing);
        }
    }
}