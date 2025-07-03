using System.Text.Json;
using System.Text.RegularExpressions;

namespace Application.Common.GeminiApi;

public class GoogleSearchBuilder
{
    private const string BaseUrl = "https://www.googleapis.com/customsearch/v1";

    private readonly string _apiKey;
    private readonly string _cx;

    public GoogleSearchBuilder()
    {
        _apiKey = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_API_KEY")
                  ?? throw new InvalidOperationException("GOOGLE_SEARCH_API_KEY is not configured");
        _cx = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_CX")
              ?? throw new InvalidOperationException("GOOGLE_SEARCH_CX is not configured");
    }

    /// <summary>
    /// Build search URL for Google Custom Search Image API
    /// </summary>
    /// <param name="query">Tên món ăn</param>
    /// <returns>URL query chuẩn hóa</returns>
    public string BuildSearchUrl(string query)
    {
        var enhancedQuery = $"{query} Vietnamese traditional dish food  authentic photo";

        return $"{BaseUrl}?" +
               $"q={Uri.EscapeDataString(enhancedQuery)}" +
               $"&searchType=image" +
               $"&imgType=photo" +
               $"&safe=active" +
               $"&imgSize=large" +
               $"&num=1" +
               $"&key={_apiKey}" +
               $"&cx={_cx}";
    }

    /// <summary>
    /// </summary>
    /// <param name="jsonContent">Content trả về</param>
    /// <param name="query">Từ khoá gốc để lọc</param>
    /// <returns>Link ảnh hoặc null</returns>
    public string? ExtractMostRelevantImageUrl(string jsonContent, string query)
    {
        using var json = JsonDocument.Parse(jsonContent);
        var root = json.RootElement;

        if (!root.TryGetProperty("items", out var items) || items.GetArrayLength() == 0)
            return null;

        string? fallbackImage = null;
        var queryLower = query.ToLower();

        foreach (var item in items.EnumerateArray())
        {
            var title = item.GetProperty("title").GetString()?.ToLower() ?? "";
            var snippet = item.TryGetProperty("snippet", out var snippetProp)
                ? snippetProp.GetString()?.ToLower() ?? ""
                : "";

            var link = item.GetProperty("link").GetString();

            if (title.Contains(queryLower) || snippet.Contains(queryLower))
                return link;

            fallbackImage ??= link;
        }

        return fallbackImage;
    }
}