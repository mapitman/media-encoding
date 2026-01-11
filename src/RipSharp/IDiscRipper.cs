namespace RipSharp;

public interface IDiscRipper
{
    Task<List<string>> ProcessDiscAsync(RipOptions options);
}
