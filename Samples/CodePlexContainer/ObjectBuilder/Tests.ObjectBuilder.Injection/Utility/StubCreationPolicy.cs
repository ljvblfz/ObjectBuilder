using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class StubCreationPolicy : ICreationPolicy
    {
        public bool Create__Called;
        public object Create__Result;
        public object Create_BuildKey;
        public IBuilderContext Create_Context;

        public ConstructorInfo GetConstructor__Result;
        public object GetConstructor_BuildKey;
        public bool GetConstructor_Called;
        public IBuilderContext GetConstructor_Context;

        public List<object> GetParameters__Result;
        public bool GetParameters_Called;
        public ConstructorInfo GetParameters_Constructor;
        public IBuilderContext GetParameters_Context;

        bool supportsReflection;

        public bool SupportsReflection
        {
            get { return supportsReflection; }
            set { supportsReflection = value; }
        }

        public object Create(IBuilderContext context,
                             object buildKey)
        {
            Create__Called = true;
            Create_Context = context;
            Create_BuildKey = buildKey;

            return Create__Result;
        }

        public ConstructorInfo GetConstructor(IBuilderContext context,
                                              object buildKey)
        {
            GetConstructor_Called = true;
            GetConstructor_Context = context;
            GetConstructor_BuildKey = buildKey;

            return GetConstructor__Result;
        }

        public object[] GetParameters(IBuilderContext context,
                                      ConstructorInfo constructor)
        {
            GetParameters_Called = true;
            GetParameters_Context = context;
            GetParameters_Constructor = constructor;

            return GetParameters__Result.ToArray();
        }
    }
}