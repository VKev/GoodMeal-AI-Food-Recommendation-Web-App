using System.Text.Json;

namespace Application.Common.GeminiApi;

public class GoogleSearchBuilder
{
    private const string BaseUrl = "https://www.googleapis.com/customsearch/v1";

    private readonly string _apiKey;
    private readonly string _cx;
    private readonly List<string> _blacklistedDomains = new()
    {
        "tiktok.com",
        "instagram.com",
        "pinterest.com"
    };

    public GoogleSearchBuilder()
    {
        _apiKey = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_API_KEY")
                  ?? throw new InvalidOperationException("GOOGLE_SEARCH_API_KEY is not configured");
        _cx = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_CX")
              ?? throw new InvalidOperationException("GOOGLE_SEARCH_CX is not configured");
    }

    public string BuildSearchUrl(string query, int num = 5)
    {
        var enhancedQuery = $"{query} món ăn Việt Nam dish Vietnamese food";

        return $"{BaseUrl}?" +
               $"q={Uri.EscapeDataString(enhancedQuery)}" +
               $"&searchType=image" +
               $"&imgType=photo" +
               $"&imgSize=medium" +
               $"&fileType=jpg,png,webp" +
               $"&safe=active" +
               $"&num={num}" +
               $"&fields=items(link)" +
               $"&key={_apiKey}" +
               $"&cx={_cx}";
    }

    public string? ExtractImageUrl(string jsonContent)
    {
        try
        {
            using var json = JsonDocument.Parse(jsonContent);
            var root = json.RootElement;

            if (root.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
            {
                foreach (var item in items.EnumerateArray())
                {
                    var link = item.GetProperty("link").GetString();
                    if (link is null) continue;
                    
                    if (IsBlacklisted(link)) continue;

                    return link;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ExtractImageUrl] Error parsing JSON: {ex.Message}");
        }

        return null;
    }

    private bool IsBlacklisted(string url)
    {
        return _blacklistedDomains.Any(domain => url.Contains(domain, StringComparison.OrdinalIgnoreCase));
    }
}
