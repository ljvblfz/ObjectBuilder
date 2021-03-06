using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class ParameterCollection : IParameterCollection
    {
        readonly List<ArgumentInfo> argumentInfo;
        readonly object[] arguments;

        public ParameterCollection(object[] arguments,
                                   ParameterInfo[] argumentInfo)
            : this(arguments, argumentInfo, delegate
                                            {
                                                return true;
                                            }) {}

        protected ParameterCollection(object[] arguments,
                                      ParameterInfo[] argumentInfo,
                                      Predicate<ParameterInfo> isArgumentPartOfCollection)
        {
            this.arguments = arguments;
            this.argumentInfo = new List<ArgumentInfo>();

            for (int idx = 0; idx < argumentInfo.Length; ++idx)
                if (isArgumentPartOfCollection(argumentInfo[idx]))
                    this.argumentInfo.Add(new ArgumentInfo(idx, argumentInfo[idx]));
        }

        public object this[int index]
        {
            get { return arguments[argumentInfo[index].Index]; }
            set { arguments[argumentInfo[index].Index] = value; }
        }

        public object this[string paramName]
        {
            get { return arguments[IndexForParameterName(paramName)]; }
            set { arguments[IndexForParameterName(paramName)] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < argumentInfo.Count; ++i)
                yield return arguments[argumentInfo[i].Index];
        }

        int IndexForParameterName(string paramName)
        {
            for (int i = 0; i < argumentInfo.Count; ++i)
                if (argumentInfo[i].Name == paramName)
                    return argumentInfo[i].Index;

            throw new ArgumentException("Invalid parameter Name", "paramName");
        }

        struct ArgumentInfo
        {
            public readonly int Index;
            public readonly string Name;

            public ArgumentInfo(int index,
                                ParameterInfo parameterInfo)
            {
                Index = index;
                Name = parameterInfo.Name;
            }
        }
    }
}