using System.Text.Json.Serialization;

namespace Application.Common.Dtos;

public class GeocodingAddressDto
{
    [JsonPropertyName("address")]
    public string? Address { get; set; }
    
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
    
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }


}