using System;
using System.Collections.Generic;

namespace ObjectBuilder
{
    public abstract class BuilderStrategy : IBuilderStrategy
    {
        public virtual object BuildUp(IBuilderContext context,
                                      object buildKey,
                                      object existing)
        {
            IBuilderStrategy next = context.GetNextInChain(this);

            if (next != null)
                return next.BuildUp(context, buildKey, existing);

            return existing;
        }

        public static Type GetTypeFromBuildKey(object buildKey)
        {
            Type result;

            if (!TryGetTypeFromBuildKey(buildKey, out result))
                throw new ArgumentException("Cannot extract type from build key " + buildKey, "buildKey");

            return result;
        }

        public static string ParametersToTypeList(params object[] parameters)
        {
            List<string> types = new List<string>();

            foreach (object parameter in parameters)
                types.Add(parameter.GetType().Name);

            return string.Join(", ", types.ToArray());
        }

        public virtual object TearDown(IBuilderContext context,
                                       object item)
        {
            IBuilderStrategy next = context.GetNextInChain(this);

            if (next != null)
                return next.TearDown(context, item);

            return item;
        }

        public static bool TryGetTypeFromBuildKey(object buildKey,
                                                  out Type type)
        {
            type = buildKey as Type;

            if (type == null)
            {
                ITypeBasedBuildKey typeBasedBuildKey = buildKey as ITypeBasedBuildKey;
                if (typeBasedBuildKey != null)
                    type = typeBasedBuildKey.Type;
            }

            return type != null;
        }
    }
}