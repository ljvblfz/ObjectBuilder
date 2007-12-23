using System.Collections;

namespace ObjectBuilder
{
    public interface IParameterCollection : IEnumerable
    {
        object this[string name] { get; set; }
        object this[int idx] { get; set; }
    }
}