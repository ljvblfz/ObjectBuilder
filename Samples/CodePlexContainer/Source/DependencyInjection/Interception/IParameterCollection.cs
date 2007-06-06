using System.Collections;

namespace CodePlex.DependencyInjection
{
    public interface IParameterCollection : IEnumerable
    {
        object this[string name] { get; set; }
        object this[int idx] { get; set; }
    }
}