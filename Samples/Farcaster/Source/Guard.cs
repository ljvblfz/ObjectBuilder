using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Farcaster
{
	internal static class Guard
	{
		public static void TypeIsAssignableFromType(Type assignee, Type providedType, Type classBeingBuilt)
		{
			if (!assignee.IsAssignableFrom(providedType))
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
					Properties.Resources.TypeNotCompatible, assignee, providedType, classBeingBuilt));
		}

		public static void ArgumentNotNull(object value, string argumentName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}
	}
}
