using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using Domain.Common.Dtos;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

public record SearchNearbyPlacesCommand(
    string Query,
    double Lat,
    double Lng,
    int Limit = 20,
    int Offset = 0,
    string Country = "vn",
    string Lang = "vi",
    int Zoom = 14)
    : ICommand;

public class SearchNearbyPlacesCommandHandler : ICommandHandler<SearchNearbyPlacesCommand>
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SearchNearbyPlacesCommandHandler(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Result> Handle(SearchNearbyPlacesCommand request, CancellationToken cancellationToken)
    {
        var baseUrl = "https://maps-data.p.rapidapi.com";
        var apiKey = Environment.GetEnvironmentVariable("RAPID_API_KEY");
        var host = "maps-data.p.rapidapi.com";

        var url = $"{baseUrl}/nearby.php?query={Uri.EscapeDataString(request.Query)}&lat={request.Lat}&lng={request.Lng}&limit={request.Limit}&offset={request.Offset}&country={request.Country}&lang={request.Lang}&zoom={request.Zoom}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Add("x-rapidapi-key", apiKey);
        httpRequest.Headers.Add("x-rapidapi-host", host);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"RapidAPI error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()} ");

        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        var results = json?["data"]?.AsArray();

        var restaurantResponses = results?.Select(item => new NearbyPlaceDto
        {
            BusinessId = item?["business_id"]?.ToString(),
            Name = item?["name"]?.ToString(),
            FullAddress = item?["full_address"]?.ToString(),
            PhoneNumber = item?["phone_number"]?.ToString(),
            Latitude = item?["latitude"]?.GetValue<double>() ?? 0,
            Longitude = item?["longitude"]?.GetValue<double>() ?? 0,
            Rating = item?["rating"]?.GetValue<double>() ?? 0,
            ReviewCount = item?["review_count"]?.GetValue<int>() ?? 0,
            PlaceLink = item?["place_link"]?.ToString(),
            PlaceId = item?["place_id"]?.ToString(),
            Website = item?["website"]?.ToString(),
            Timezone = item?["timezone"]?.ToString(),
            City = item?["city"]?.ToString(),
            State = item?["state"]?.ToString(),
            PriceLevel = int.TryParse(item?["price_level"]?.ToString(), out var priceLevelParsed)
                ? priceLevelParsed
                : (int?)null, 
            Types = item?["types"] is JsonArray typeArray
                ? typeArray.Select(t => t?.ToString() ?? "").ToList()
                : new(),

            Description = item?["description"] is JsonArray descArray
                ? descArray.Select(d => d?.ToString() ?? "").ToList()
                : new(),

            Photos = item?["photos"] is JsonArray photoArray
                ? photoArray.Select(p => {
                    if (p is not JsonObject photoObj) return null;

                    return new PlacePhotoDto
                    {
                        Src = photoObj["src"]?.ToString(),
                        MaxWidth = photoObj["max_size"]?[0]?.GetValue<int>() ?? 0,
                        MaxHeight = photoObj["max_size"]?[1]?.GetValue<int>() ?? 0,
                        MinWidth = photoObj["min_size"]?[0]?.GetValue<int>() ?? 0,
                        MinHeight = photoObj["min_size"]?[1]?.GetValue<int>() ?? 0,
                    };
                }).Where(p => p != null).ToList()
                : new(),

            WorkingHours = item?["working_hours"] is JsonObject whObj
                ? whObj.ToDictionary(
                    wh => wh.Key,
                    wh => wh.Value?.AsArray()?.Select(v => v?.ToString() ?? "").ToList() ?? new List<string>()
                )
                : new()
        }).ToList() ?? new();

        return Result.Success(restaurantResponses);
    }
}