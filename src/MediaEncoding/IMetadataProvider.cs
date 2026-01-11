using System.Threading.Tasks;

namespace MediaEncoding;

public interface IMetadataProvider
{
    string Name { get; }
    Task<Metadata?> LookupAsync(string title, bool isTv, int? year);
}
