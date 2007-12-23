using System.Collections.Generic;

namespace ObjectBuilder
{
    public class PropertySetterPolicy : IPropertySetterPolicy
    {
        readonly List<IPropertySetterInfo> properties = new List<IPropertySetterInfo>();

        public List<IPropertySetterInfo> Properties
        {
            get { return properties; }
        }
    }
}