using System.Reflection;

namespace ObjectBuilder
{
    public class ReflectionPropertySetterInfo : IPropertySetterInfo
    {
        readonly PropertyInfo property;
        readonly IParameter value;

        public ReflectionPropertySetterInfo(PropertyInfo property,
                                            IParameter value)
        {
            this.property = property;
            this.value = value;
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public IParameter Value
        {
            get { return value; }
        }

        public void Set(IBuilderContext context,
                        object instance,
                        object buildKey)
        {
            Property.SetValue(instance, Value.GetValue(context), null);
        }
    }
}