using System;
using System.Reflection;

namespace ObjectBuilder
{
    public class DefaultCreationPolicy : ICreationPolicy
    {
        public bool SupportsReflection
        {
            get { return true; }
        }

        public object Create(IBuilderContext context,
                             object buildKey)
        {
            Type typeToBuild = GetTypeFromBuildKey(buildKey);
            if (typeToBuild == null)
                throw new ArgumentException("Default creation policy cannot create for unknown build key " + buildKey, "buildKey");

            ConstructorInfo[] constructors = typeToBuild.GetConstructors();
            if (constructors.Length == 0)
                return Activator.CreateInstance(typeToBuild);

            ConstructorInfo constructor = GetConstructor(typeToBuild);
            return constructor.Invoke(GetParameters(context, constructor));
        }

        protected virtual object GetBuildKeyFromType(Type type)
        {
            return type;
        }

        public ConstructorInfo GetConstructor(IBuilderContext context,
                                              object buildKey)
        {
            Type typeToBuild = GetTypeFromBuildKey(buildKey);
            if (typeToBuild == null)
                throw new ArgumentException("Default creation policy cannot create for unknown build key " + buildKey, "buildKey");

            return GetConstructor(typeToBuild);
        }

        static ConstructorInfo GetConstructor(Type typeToBuild)
        {
            ConstructorInfo[] constructors = typeToBuild.GetConstructors();

            if (constructors.Length > 0)
                return constructors[0];

            return null;
        }

        public object[] GetParameters(IBuilderContext context,
                                      ConstructorInfo constructor)
        {
            ParameterInfo[] parms = constructor.GetParameters();
            object[] parmsValueArray = new object[parms.Length];

            for (int i = 0; i < parms.Length; ++i)
                parmsValueArray[i] = context.HeadOfChain.BuildUp(context,
                                                                 GetBuildKeyFromType(parms[i].ParameterType),
                                                                 null);

            return parmsValueArray;
        }

        protected virtual Type GetTypeFromBuildKey(object buildKey)
        {
            return buildKey as Type;
        }
    }
}