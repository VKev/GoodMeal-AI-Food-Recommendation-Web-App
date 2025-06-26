using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using Application.Common.Dtos;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

public record SearchPlaceReviewsCommand(
    string Business_id,
    string Place_id,
    string Country = "vn",
    string Lang = "vi",
    int limit = 15,
    string cursor = "",
    string sort = "Relevant")
    : ICommand;

public class SearchPlaceReviewsCommandHandler : ICommandHandler<SearchPlaceReviewsCommand>
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SearchPlaceReviewsCommandHandler(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Result> Handle(SearchPlaceReviewsCommand request, CancellationToken cancellationToken)
    {
        var baseUrl = "https://maps-data.p.rapidapi.com";
        var apiKey = Environment.GetEnvironmentVariable("RAPID_API_KEY");
        var host = "maps-data.p.rapidapi.com";

        var url =
            $"{baseUrl}/reviews.php?business_id={Uri.EscapeDataString(request.Business_id)}&country={request.Country}&lang={request.Lang}&limit={request.limit}&cursor={request.cursor}&sort={request.sort}&place_id={request.Place_id}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Add("x-rapidapi-key", apiKey);
        httpRequest.Headers.Add("x-rapidapi-host", host);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"RapidAPI error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()} ");

        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        var reviewsNode = json?["data"]?["reviews"] as JsonArray;
        var reviews = new List<PlaceReviewsDto>();

        if (reviewsNode != null)
        {
            foreach (var item in reviewsNode)
            {
                if (item is not JsonObject reviewObj) continue;

                var review = new PlaceReviewsDto
                {
                    UserName = reviewObj["user_name"]?.ToString(),
                    UserTotalReviews = reviewObj["user_total_reviews"]?.GetValue<int>() ?? 0,
                    UserTotalPhotos = reviewObj["user_total_photos"]?.GetValue<int>() ?? 0,
                    UserAvatar = reviewObj["user_avatar"]?.ToString(),
                    UserLink = reviewObj["user_link"]?.ToString(),
                    IsoDate = reviewObj["iso_date"]?.ToString(),
                    IsoDateTimestamp = reviewObj["iso_date_timestamp"]?.GetValue<long>() ?? 0,
                    IsoDateOfLastEdit = reviewObj["iso_date_of_last_edit"]?.ToString(),
                    IsoDateOfLastEditTimestamp = reviewObj["iso_date_of_last_edit_timestamp"]?.GetValue<long>() ?? 0,
                    ReviewId = reviewObj["review_id"]?.ToString(),
                    ReviewTime = reviewObj["review_time"]?.ToString(),
                    ReviewTimestamp = reviewObj["review_timestamp"]?.GetValue<long>() ?? 0,
                    ReviewLink = reviewObj["review_link"]?.ToString(),
                    ReviewText = reviewObj["review_text"]?.ToString(),
                    ReviewPhotos = reviewObj["review_photos"] is JsonArray photoArray
                        ? photoArray.Select(p => p?.ToString() ?? "").ToList()
                        : new List<string>(),
                    BusinessResponseText = reviewObj["business_response_text"]?.ToString(),
                    ReviewServices = reviewObj["review_services"] is JsonObject rsObj
                        ? rsObj.ToDictionary(x => x.Key, x => (object)(x.Value?.ToString() ?? ""))
                        : new Dictionary<string, object>(),
                    Translations = reviewObj["translations"] is JsonObject transObj
                        ? transObj.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? "")
                        : new Dictionary<string, string>(),
                    ReviewRate = reviewObj["review_rate"]?.GetValue<int>() ?? 0,
                    ReviewCursor = reviewObj["review_cursor"]?.ToString()
                };

                reviews.Add(review);
            }
        }

        return Result.Success(reviews);
    }
}