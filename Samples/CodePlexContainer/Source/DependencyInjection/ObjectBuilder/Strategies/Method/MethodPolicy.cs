using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class MethodPolicy : IMethodPolicy
    {
        // Fields

        Dictionary<string, IMethodCallInfo> methods = new Dictionary<string, IMethodCallInfo>();

        // Properties

        public Dictionary<string, IMethodCallInfo> Methods
        {
            get { return methods; }
        }
    }
}