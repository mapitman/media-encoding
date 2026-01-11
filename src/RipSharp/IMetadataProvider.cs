using System.Threading.Tasks;

namespace RipSharp;

public interface IMetadataProvider
{
    string Name { get; }
    Task<Metadata?> LookupAsync(string title, bool isTv, int? year);
}
