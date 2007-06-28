using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodReturn
    {
        Exception Exception { get; set; }
        IParameterCollection Outputs { get; }
        object ReturnValue { get; set; }
    }
}