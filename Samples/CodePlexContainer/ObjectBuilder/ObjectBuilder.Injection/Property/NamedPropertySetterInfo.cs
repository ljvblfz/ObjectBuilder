using System.Reflection;

namespace ObjectBuilder
{
    public class NamedPropertySetterInfo : IPropertySetterInfo
    {
        readonly string propertyName;
        readonly IParameter value;

        public NamedPropertySetterInfo(string propertyName,
                                       IParameter value)
        {
            this.propertyName = propertyName;
            this.value = value;
        }

        public void Set(IBuilderContext context,
                        object instance,
                        object buildKey)
        {
            PropertyInfo property = instance.GetType().GetProperty(propertyName);
            property.SetValue(instance, value.GetValue(context), null);
        }
    }
}