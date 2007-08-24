using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public static class DependencyResolver
    {
        public static object Resolve(IBuilderContext context,
                                     object buildKey,
                                     NotPresentBehavior behavior)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(buildKey, "buildKey");

            if (context.Locator.Contains(buildKey))
                return context.Locator.Get(buildKey);

            switch (behavior)
            {
                case NotPresentBehavior.Build:
                    return context.HeadOfChain.BuildUp(context, buildKey, null);

                case NotPresentBehavior.Null:
                    return null;

                case NotPresentBehavior.Throw:
                    throw new DependencyMissingException("Could not locate dependency " + buildKey);

                default:
                    throw new InvalidOperationException("Unknown NotPresentBehavior " + behavior);
            }
        }
    }
}