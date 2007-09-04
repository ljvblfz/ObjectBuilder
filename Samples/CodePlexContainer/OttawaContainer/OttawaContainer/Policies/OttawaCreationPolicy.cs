using System;
using System.Reflection;
using ObjectBuilder;

namespace Ottawa
{
    public class OttawaCreationPolicy : ICreationPolicy
    {
        public bool SupportsReflection
        {
            get { return true; }
        }

        public object Create(IBuilderContext context,
                             object buildKey)
        {
            Type typeToBuild = buildKey as Type;
            //if (typeToBuild == null)
            //    throw new ArgumentException("Default creation policy cannot create for unknown build key " + buildKey, "buildKey");

            ConstructorInfo constructor = GetConstructor(typeToBuild);
            if (constructor == null)
                throw new ComponentActivatorException(typeToBuild);

            return constructor.Invoke(GetParameters(context, constructor));
        }

        public ConstructorInfo GetConstructor(IBuilderContext context,
                                              object buildKey)
        {
            //Type typeToBuild = GetTypeFromBuildKey(buildKey);
            //if (typeToBuild == null)
            //    throw new ArgumentException("Default creation policy cannot create for unknown build key " + buildKey, "buildKey");

            //return GetConstructor(typeToBuild);

            throw new NotImplementedException();
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
                parmsValueArray[i] = context.HeadOfChain.BuildUp(context, parms[i].ParameterType, null);

            return parmsValueArray;
        }
    }
}