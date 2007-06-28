using System;
using System.Globalization;
using System.Reflection;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PropertySetterStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            if (existing != null)
                InjectProperties(context, existing, idToBuild);

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        static void InjectProperties(IBuilderContext context,
                                     object obj,
                                     string id)
        {
            if (obj == null)
                return;

            Type type = obj.GetType();
            IPropertySetterPolicy policy = context.Policies.Get<IPropertySetterPolicy>(type, id);

            if (policy == null)
                return;

            foreach (IPropertySetterInfo propSetterInfo in policy.Properties.Values)
            {
                PropertyInfo propInfo = propSetterInfo.SelectProperty(context, type, id);

                if (propInfo != null)
                {
                    if (propInfo.CanWrite)
                    {
                        object value = propSetterInfo.GetValue(context, type, id, propInfo);

                        if (value != null)
                            Guard.TypeIsAssignableFromType(propInfo.PropertyType, value.GetType(), obj.GetType());

                        propInfo.SetValue(obj, value, null);
                    }
                    else
                        throw new ArgumentException(String.Format(
                                                        CultureInfo.CurrentCulture,
                                                        Resources.CannotInjectReadOnlyProperty,
                                                        type, propInfo.Name));
                }
            }
        }
    }
}