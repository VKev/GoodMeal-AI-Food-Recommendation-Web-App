using System.Text.Json.Serialization;

namespace Domain.Common.Dtos
{
    public class PlaceDetailsDto
    {
        [JsonPropertyName("business_id")]
        public string BusinessId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("full_address")]
        public string FullAddress { get; set; }
        
        [JsonPropertyName("full_address_array")]
        public List<string> FullAddressArray { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("review_count")]
        public int ReviewCount { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("website")]
        public string Website { get; set; }

        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }

        [JsonPropertyName("place_link")]
        public string PlaceLink { get; set; }

        [JsonPropertyName("types")]
        public List<string> Types { get; set; }

        [JsonPropertyName("price_level")]
        public string PriceLevel { get; set; }
        
        [JsonPropertyName("plus_code")]
        public string PlusCode { get; set; }

        [JsonPropertyName("cid")]
        public string Cid { get; set; }

        [JsonPropertyName("is_claimed")]
        public bool IsClaimed { get; set; }

        [JsonPropertyName("working_hours")]
        public Dictionary<string, List<string>> WorkingHours { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }
        
        [JsonPropertyName("description")]
        public List<string> Description { get; set; } = new();

        [JsonPropertyName("details")]
        public Dictionary<string, List<string>> Details { get; set; }

        [JsonPropertyName("photos")]
        public List<PlacePhotoDto> Photos { get; set; }
        
    }
}