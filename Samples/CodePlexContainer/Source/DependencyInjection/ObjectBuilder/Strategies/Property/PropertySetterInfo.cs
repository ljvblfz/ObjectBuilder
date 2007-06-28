using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PropertySetterInfo : IPropertySetterInfo
    {
        readonly string name = null;
        readonly PropertyInfo prop = null;
        readonly IParameter value = null;

        public PropertySetterInfo(string name,
                                  IParameter value)
        {
            this.name = name;
            this.value = value;
        }

        public PropertySetterInfo(PropertyInfo propInfo,
                                  IParameter value)
        {
            prop = propInfo;
            this.value = value;
        }

        public object GetValue(IBuilderContext context,
                               Type type,
                               string id,
                               PropertyInfo propInfo)
        {
            return value.GetValue(context);
        }

        public PropertyInfo SelectProperty(IBuilderContext context,
                                           Type type,
                                           string id)
        {
            if (prop != null)
                return prop;

            return type.GetProperty(name);
        }
    }
}