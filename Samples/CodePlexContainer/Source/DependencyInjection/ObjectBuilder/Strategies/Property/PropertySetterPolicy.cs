using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PropertySetterPolicy : IPropertySetterPolicy
    {
        readonly Dictionary<string, IPropertySetterInfo> properties = new Dictionary<string, IPropertySetterInfo>();

        public Dictionary<string, IPropertySetterInfo> Properties
        {
            get { return properties; }
        }
    }
}