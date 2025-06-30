using System.Text.Json.Serialization;

namespace Application.Common.Dtos;

public class PlaceReviewsDto
{
    [JsonPropertyName("user_name")]
    public string UserName { get; set; }

    [JsonPropertyName("user_total_reviews")]
    public int UserTotalReviews { get; set; }

    [JsonPropertyName("user_total_photos")]
    public int UserTotalPhotos { get; set; }

    [JsonPropertyName("user_avatar")]
    public string UserAvatar { get; set; }

    [JsonPropertyName("user_link")]
    public string UserLink { get; set; }

    [JsonPropertyName("iso_date")]
    public string IsoDate { get; set; }

    [JsonPropertyName("iso_date_timestamp")]
    public long IsoDateTimestamp { get; set; }

    [JsonPropertyName("iso_date_of_last_edit")]
    public string IsoDateOfLastEdit { get; set; }

    [JsonPropertyName("iso_date_of_last_edit_timestamp")]
    public long IsoDateOfLastEditTimestamp { get; set; }

    [JsonPropertyName("review_id")]
    public string ReviewId { get; set; }

    [JsonPropertyName("review_time")]
    public string ReviewTime { get; set; }

    [JsonPropertyName("review_timestamp")]
    public long ReviewTimestamp { get; set; }

    [JsonPropertyName("review_link")]
    public string ReviewLink { get; set; }

    [JsonPropertyName("review_text")]
    public string ReviewText { get; set; }

    [JsonPropertyName("review_photos")]
    public List<string> ReviewPhotos { get; set; }

    [JsonPropertyName("business_response_text")]
    public string BusinessResponseText { get; set; }

    [JsonPropertyName("review_services")]
    public Dictionary<string, object> ReviewServices { get; set; }

    [JsonPropertyName("translations")]
    public Dictionary<string, string> Translations { get; set; }

    [JsonPropertyName("review_rate")]
    public int ReviewRate { get; set; }

    [JsonPropertyName("review_cursor")]
    public string ReviewCursor { get; set; }
}