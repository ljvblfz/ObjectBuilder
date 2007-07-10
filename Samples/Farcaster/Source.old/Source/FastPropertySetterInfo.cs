using System;
using Microsoft.Practices.ObjectBuilder;
using System.Reflection;

namespace Farcaster
{
	/// <summary>
	/// Implementation of <see cref="IPropertySetterInfo"/> that uses and caches 
	/// instances of <see cref="FastPropertyInfo"/> for performant property 
	/// setting and retrieval.
	/// </summary>
	public class FastPropertySetterInfo : IPropertySetterInfo
	{
		string name = null;
		bool propertyNotFound;
		PropertyInfo prop = null;
		IParameter value = null;

		/// <summary>
		/// Initializes a new instance of <see cref="PropertySetterInfo"/> using the provided
		/// property name and value.
		/// </summary>
		/// <param name="name">The name of the property to be set.</param>
		/// <param name="value">The value to be set into the property.</param>
		public FastPropertySetterInfo(string name, IParameter value)
		{
			this.name = name;
			this.value = value;
		}

		/// <summary>
		/// Instantiates a new instance of <see cref="PropertySetterInfo"/> using the provided
		/// <see cref="PropertyInfo"/> and value.
		/// </summary>
		/// <param name="propInfo">The property to be set.</param>
		/// <param name="value">The value to set into the property.</param>
		public FastPropertySetterInfo(PropertyInfo propInfo, IParameter value)
		{
			this.prop = propInfo;
			this.value = value;
		}

		/// <summary>
		/// See <see cref="IPropertySetterInfo.SelectProperty"/> for more information.
		/// </summary>
		public PropertyInfo SelectProperty(IBuilderContext context, Type type, string id)
		{
			if (prop != null) return prop;
			if (propertyNotFound) return null;

			PropertyInfo property = type.GetProperty(name);
			if (property == null)
			{
				propertyNotFound = true;
				return null;
			}

			prop = new FastPropertyInfo(property);

			return prop;
		}

		/// <summary>
		/// See <see cref="IPropertySetterInfo.GetValue"/> for more information.
		/// </summary>
		public object GetValue(IBuilderContext context, Type type, string id, PropertyInfo propInfo)
		{
			return value.GetValue(context);
		}
	}
}
