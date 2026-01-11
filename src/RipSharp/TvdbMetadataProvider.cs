using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RipSharp;

/// <summary>
/// TVDB metadata provider using raw REST calls. Currently scaffolding the flow; real
/// episode/series title lookups will be added in follow-up work for issue #37.
/// </summary>
public class TvdbMetadataProvider : IMetadataProvider
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly IProgressNotifier _notifier;

    public string Name => "TVDB";

    public TvdbMetadataProvider(HttpClient http, string apiKey, IProgressNotifier notifier)
    {
        _http = http;
        _apiKey = apiKey;
        _notifier = notifier;
    }

    public async Task<Metadata?> LookupAsync(string title, bool isTv, int? year)
    {
        // TVDB focus is TV content; for movies keep existing providers as primary.
        if (!isTv)
            return null;

        try
        {
            // Placeholder: use TVDB v4 search endpoint once token flow is wired.
            // We intentionally return null for now to fall back to existing providers
            // while we complete the TVDB authentication and episode-title mapping.
            _ = title;
            _ = year;
            await Task.CompletedTask;
        }
        catch
        {
            // Swallow and fall back to other providers
        }

        return null;
    }
}
