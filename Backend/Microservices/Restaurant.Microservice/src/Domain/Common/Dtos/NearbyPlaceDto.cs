using System.Text.Json.Serialization;

namespace Domain.Common.Dtos;

public class NearbyPlaceDto
{
    [JsonPropertyName("business_id")]
    public string? BusinessId { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("full_address")]
    public string? FullAddress { get; set; }
    
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
    
    [JsonPropertyName("rating")]
    public double Rating { get; set; }
    
    [JsonPropertyName("review_count")]
    public int ReviewCount { get; set; }
    
    [JsonPropertyName("place_link")]
    public string? PlaceLink { get; set; }
    
    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }
    
    [JsonPropertyName("website")]
    public string? Website { get; set; }
    
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
    
    [JsonPropertyName("city")]
    public string? City { get; set; }
    
    [JsonPropertyName("state")]
    public string? State { get; set; }
    
    [JsonPropertyName("price_level")]
    public int? PriceLevel { get; set; }
    
    [JsonPropertyName("types")]
    public List<string> Types { get; set; } = new();
    
    [JsonPropertyName("description")]
    public List<string> Description { get; set; } = new();
    
    [JsonPropertyName("photos")]
    public List<PlacePhotoDto> Photos { get; set; } = new();
    
    [JsonPropertyName("working_hours")]
    public Dictionary<string, List<string>> WorkingHours { get; set; } = new();
}