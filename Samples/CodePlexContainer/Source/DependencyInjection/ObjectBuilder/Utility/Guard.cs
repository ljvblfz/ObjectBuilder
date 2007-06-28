using System;
using System.Globalization;
using System.Reflection;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    static class Guard
    {
        public static void ArgumentNotNull(object argumentValue,
                                           string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }

        public static void ArgumentNotNullOrEmptyString(string argumentValue,
                                                        string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeEmpty, argumentName));
        }

        public static void EnumValueIsDefined(Type enumType,
                                              object value,
                                              string argumentName)
        {
            if (Enum.IsDefined(enumType, value) == false)
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                          Resources.InvalidEnumValue,
                                                          argumentName, enumType));
        }

        public static void TypeIsAssignableFromType(Type assignee,
                                                    Type providedType,
                                                    string argumentName)
        {
            if (!providedType.IsAssignableFrom(assignee))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                          Resources.TypeNotCompatible, assignee, providedType), argumentName);
        }

        public static void TypeIsAssignableFromType(Type assignee,
                                                    Type providedType,
                                                    Type classBeingBuilt)
        {
            if (!assignee.IsAssignableFrom(providedType))
                throw new IncompatibleTypesException(string.Format(CultureInfo.CurrentCulture,
                                                                   Resources.TypeNotCompatible, assignee, providedType, classBeingBuilt));
        }

        public static void ValidateMethodParameters(MethodBase methodInfo,
                                                    object[] parameters,
                                                    Type typeBeingBuilt)
        {
            ParameterInfo[] paramInfos = methodInfo.GetParameters();

            for (int i = 0; i < paramInfos.Length; i++)
                if (parameters[i] != null)
                    TypeIsAssignableFromType(paramInfos[i].ParameterType, parameters[i].GetType(), typeBeingBuilt);
        }
    }
}