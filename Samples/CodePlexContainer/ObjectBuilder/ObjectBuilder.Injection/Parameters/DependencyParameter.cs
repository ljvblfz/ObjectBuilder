using System;

namespace ObjectBuilder
{
    public class DependencyParameter : IParameter
    {
        readonly object buildKey;
        readonly NotPresentBehavior notPresentBehavior;
        object value = null;

        public DependencyParameter(object buildKey,
                                   NotPresentBehavior notPresentBehavior)
        {
            this.buildKey = buildKey;
            this.notPresentBehavior = notPresentBehavior;
        }

        public object BuildKey
        {
            get { return buildKey; }
        }

        public NotPresentBehavior NotPresentBehavior
        {
            get { return notPresentBehavior; }
        }

        public Type GetParameterType(IBuilderContext context)
        {
            return GetValue(context).GetType();
        }

        public object GetValue(IBuilderContext context)
        {
            if (value == null)
                value = DependencyResolver.Resolve(context, BuildKey, NotPresentBehavior);

            return value;
        }
    }
}