namespace Application.Common.GeminiApi;

public class GeminiResponse
{
    public string Error { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<FoodItem> Foods { get; set; } = new();
    public List<string> FoodNames { get; set; } = new();
    public string? Location { get; set; }
}