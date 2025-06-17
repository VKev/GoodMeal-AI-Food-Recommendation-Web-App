using System.Text.Json;

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
        // Thêm keyword để tăng độ chính xác ảnh món ăn
        var enhancedQuery = $"{query} món ăn Việt Nam dish Vietnamese food";

        return $"{BaseUrl}?" +
               $"q={Uri.EscapeDataString(enhancedQuery)}" +
               $"&searchType=image" +
               $"&imgType=photo" +
               $"&safe=active" +
               $"&num=1" +
               $"&key={_apiKey}" +
               $"&cx={_cx}";
    }

    /// <summary>
    /// Extract ảnh đầu tiên từ JSON response của Google Image Search
    /// </summary>
    /// <param name="jsonContent">Content trả về</param>
    /// <returns>Link ảnh hoặc null</returns>
    public string? ExtractImageUrl(string jsonContent)
    {
        using var json = JsonDocument.Parse(jsonContent);
        var root = json.RootElement;

        if (root.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
        {
            return items[0].GetProperty("link").GetString();
        }

        return null;
    }
}