using System;

namespace ObjectBuilder
{
    public interface IMethodReturn
    {
        Exception Exception { get; set; }
        IParameterCollection Outputs { get; }
        object ReturnValue { get; set; }
    }
}