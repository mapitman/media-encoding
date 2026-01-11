using System.Threading.Tasks;

namespace RipSharp.Abstractions;

public interface IMetadataProvider
{
    string Name { get; }
    Task<MetadataInfo?> LookupAsync(string title, bool isTv, int? year);
}
