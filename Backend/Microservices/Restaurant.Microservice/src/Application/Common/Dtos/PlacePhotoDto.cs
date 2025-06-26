using System.Text.Json.Serialization;

namespace Application.Common.Dtos;

public class PlacePhotoDto
{
    [JsonPropertyName("src")]
    public string? Src { get; set; }
    [JsonPropertyName("max_width")]
    public int? MaxWidth { get; set; }
    [JsonPropertyName("max_height")]
    public int? MaxHeight { get; set; }
    [JsonPropertyName("min_width")]
    public int? MinWidth { get; set; }
    [JsonPropertyName("min_height")]
    public int? MinHeight { get; set; }
}