using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderAwareStrategy : BuilderStrategy
    {
        // Methods

        public override object BuildUp(IBuilderContext context,
                                       Type t,
                                       object existing,
                                       string id)
        {
            IBuilderAware awareObject = existing as IBuilderAware;

            if (awareObject != null)
                awareObject.OnBuiltUp(id);

            return base.BuildUp(context, t, existing, id);
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