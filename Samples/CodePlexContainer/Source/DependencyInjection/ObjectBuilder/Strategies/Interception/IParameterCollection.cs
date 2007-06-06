using System.Collections;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IParameterCollection : IEnumerable
    {
        object this[string name] { get; set; }
        object this[int idx] { get; set; }
    }
}