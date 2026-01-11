namespace RipSharp;

public interface IMetadataService
{
    Task<Metadata?> LookupAsync(string title, bool isTv, int? year);
}
