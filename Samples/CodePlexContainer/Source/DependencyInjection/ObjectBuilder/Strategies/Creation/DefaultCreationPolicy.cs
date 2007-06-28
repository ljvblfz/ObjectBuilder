using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class DefaultCreationPolicy : ICreationPolicy
    {
        public object[] GetParameters(IBuilderContext context,
                                      Type type,
                                      string id,
                                      ConstructorInfo constructor)
        {
            ParameterInfo[] parms = constructor.GetParameters();
            object[] parmsValueArray = new object[parms.Length];

            for (int i = 0; i < parms.Length; ++i)
                parmsValueArray[i] = context.HeadOfChain.BuildUp(context, parms[i].ParameterType, null, id);

            return parmsValueArray;
        }

        public ConstructorInfo SelectConstructor(IBuilderContext context,
                                                 Type typeToBuild,
                                                 string idToBuild)
        {
            ConstructorInfo[] constructors = typeToBuild.GetConstructors();

            if (constructors.Length > 0)
                return constructors[0];

            return null;
        }
    }
}