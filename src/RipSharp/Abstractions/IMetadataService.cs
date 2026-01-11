namespace RipSharp.Abstractions;

public interface IMetadataService
{
    Task<MetadataInfo?> LookupAsync(string title, bool isTv, int? year);
}
