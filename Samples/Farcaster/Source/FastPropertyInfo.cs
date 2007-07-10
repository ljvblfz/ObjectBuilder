using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;

namespace Farcaster
{
	/// <summary>
	/// Custom <see cref="PropertyInfo"/> that wraps an existing property and provides 
	/// <c>Reflection.Emit</c>-generated <see cref="GetValue"/> and <see cref="SetValue"/> 
	/// implementations for drastically improved performance over default late-bind 
	/// invoke.
	/// </summary>
	public class FastPropertyInfo : PropertyInfo
	{
		delegate void SetValueDelegate(object instance, object value);
		delegate object GetValueDelegate(object instance);

		PropertyInfo property;
		SetValueDelegate setValueImpl = null;
		GetValueDelegate getValueImpl = null;

		/// <summary>
		/// Initializes the property and generates the implementation for getter and setter methods.
		/// </summary>
		public FastPropertyInfo(PropertyInfo property)
		{
			Guard.ArgumentNotNull(property, "property");
			this.property = property;

			if (property.CanWrite)
			{
				DynamicMethod dm = new DynamicMethod("SetValueImpl", null, new Type[] { typeof(object), typeof(object) }, this.GetType().Module, false);
				ILGenerator ilgen = dm.GetILGenerator();

				//L_0000: nop 
				ilgen.Emit(OpCodes.Nop);
				//L_0001: ldarg.0 
				ilgen.Emit(OpCodes.Ldarg_0);
				//L_0002: castclass [declaringType]
				ilgen.Emit(OpCodes.Castclass, property.DeclaringType);
				//L_0007: ldarg.1 
				ilgen.Emit(OpCodes.Ldarg_1);
				//L_0008: castclass [propertyType]
				ilgen.Emit(OpCodes.Castclass, property.PropertyType);
				//L_000d: callvirt instance void [instanceType]::set_[propertyName](propertyType)
				ilgen.EmitCall(OpCodes.Callvirt, property.GetSetMethod(), null);
				//L_0012: nop 
				ilgen.Emit(OpCodes.Nop);
				//L_0013: ret 
				ilgen.Emit(OpCodes.Ret);
				
				setValueImpl = (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
			}

			if (property.CanRead)
			{
				DynamicMethod dm = new DynamicMethod("GetValueImpl", typeof(object), new Type[] { typeof(object) }, this.GetType().Module, false);
				ILGenerator ilgen = dm.GetILGenerator();

				//.locals init (
				//      object obj1)
				LocalBuilder result = ilgen.DeclareLocal(typeof(object));
				//L_0000: nop 
				ilgen.Emit(OpCodes.Nop);
				//L_0001: ldarg.0 
				ilgen.Emit(OpCodes.Ldarg_0);
				//L_0002: castclass [declaringType]
				ilgen.Emit(OpCodes.Castclass, property.DeclaringType);
				//L_0007: callvirt instance [declaringType] get_[B]()
				ilgen.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);
				//L_000c: stloc.0 
				ilgen.Emit(OpCodes.Stloc_0, result);
				//L_000f: ldloc.0 
				ilgen.Emit(OpCodes.Ldloc_0);
				//L_0010: ret 
				ilgen.Emit(OpCodes.Ret);

				getValueImpl = (GetValueDelegate)dm.CreateDelegate(typeof(GetValueDelegate));
			}
		}

		/// <summary>
		/// See <see cref="PropertyInfo.SetValue(object, object, BindingFlags, Binder, object[], CultureInfo)"/>.
		/// </summary>
		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			if (CanWrite)
			{
				setValueImpl(obj, value);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// See <see cref="PropertyInfo.GetValue(object, BindingFlags, Binder, object[], CultureInfo)"/>.
		/// </summary>
		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			if (CanRead)
			{
				return getValueImpl(obj);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		#region Pass-through members

		/// <summary>
		/// See <see cref="PropertyInfo.PropertyType"/>.
		/// </summary>
		public override Type PropertyType
		{
			get { return property.PropertyType; }
		}

		/// <summary>
		/// See <see cref="MemberInfo.DeclaringType"/>.
		/// </summary>
		public override Type DeclaringType
		{
			get { return property.DeclaringType; }
		}

		/// <summary>
		/// See <see cref="PropertyInfo.Attributes"/>.
		/// </summary>
		public override PropertyAttributes Attributes
		{
			get { return property.Attributes; }
		}

		/// <summary>
		/// See <see cref="PropertyInfo.CanRead"/>.
		/// </summary>
		public override bool CanRead
		{
			get { return property.CanRead; }
		}

		/// <summary>
		/// See <see cref="PropertyInfo.CanWrite"/>.
		/// </summary>
		public override bool CanWrite
		{
			get { return property.CanWrite; }
		}

		/// <summary>
		/// See <see cref="PropertyInfo.GetAccessors(bool)"/>.
		/// </summary>
		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			return property.GetAccessors(nonPublic);
		}

		/// <summary>
		/// See <see cref="PropertyInfo.GetGetMethod(bool)"/>.
		/// </summary>
		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return property.GetGetMethod(nonPublic);
		}

		/// <summary>
		/// See <see cref="PropertyInfo.GetIndexParameters"/>.
		/// </summary>
		public override ParameterInfo[] GetIndexParameters()
		{
			return property.GetIndexParameters();
		}

		/// <summary>
		/// See <see cref="PropertyInfo.GetSetMethod(bool)"/>.
		/// </summary>
		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			return property.GetSetMethod(nonPublic);
		}

		/// <summary>
		/// See <see cref="MemberInfo.GetCustomAttributes(Type, bool)"/>.
		/// </summary>
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return property.GetCustomAttributes(attributeType, inherit);
		}

		/// <summary>
		/// See <see cref="MemberInfo.GetCustomAttributes(bool)"/>.
		/// </summary>
		public override object[] GetCustomAttributes(bool inherit)
		{
			return property.GetCustomAttributes(inherit);
		}

		/// <summary>
		/// See <see cref="MemberInfo.IsDefined"/>.
		/// </summary>
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return property.IsDefined(attributeType, inherit);
		}

		/// <summary>
		/// See <see cref="MemberInfo.Name"/>.
		/// </summary>
		public override string Name
		{
			get { return property.Name; }
		}

		/// <summary>
		/// See <see cref="MemberInfo.ReflectedType"/>.
		/// </summary>
		public override Type ReflectedType
		{
			get { return property.ReflectedType; }
		}

		#endregion
	}
}
