using RAE.Models;

namespace RAE
{
    public interface IWord : IEntry
    {
        string Origin { get; }
        string[] Values { get; }
        IDefinition[] Definitions { get; }
    }
}
