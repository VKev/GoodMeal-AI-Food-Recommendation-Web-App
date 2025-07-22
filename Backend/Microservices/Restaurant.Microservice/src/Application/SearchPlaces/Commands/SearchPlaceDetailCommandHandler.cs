using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using Domain.Common.Dtos;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

public record SearchPlaceDetailCommand(
    string Business_id,
    string Place_id,
    string Country = "vn",
    string Lang = "vi")
    : ICommand;

public class SearchPlaceDetailCommandHandler : ICommandHandler<SearchPlaceDetailCommand>
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SearchPlaceDetailCommandHandler(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Result> Handle(SearchPlaceDetailCommand request, CancellationToken cancellationToken)
    {
        var baseUrl = "https://maps-data.p.rapidapi.com";
        var apiKey = Environment.GetEnvironmentVariable("RAPID_API_KEY");
        var host = "maps-data.p.rapidapi.com";

        var url =
            $"{baseUrl}/place.php?business_id={Uri.EscapeDataString(request.Business_id)}&country={request.Country}&lang={request.Lang}&place_id={request.Place_id}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Add("x-rapidapi-key", apiKey);
        httpRequest.Headers.Add("x-rapidapi-host", host);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"RapidAPI error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()} ");

        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        var firstItem = json?["data"]?.AsArray()?.FirstOrDefault();

        if (firstItem is null)
            return Result.Failure(Error.NullValue);

// Parse thành PlaceDetailsDto
        var place = new PlaceDetailsDto
        {
            BusinessId = firstItem["business_id"]?.ToString(),
            Name = firstItem["name"]?.ToString(),
            FullAddress = firstItem["full_address"]?.ToString(),
            FullAddressArray = firstItem["full_address_array"] is JsonArray addressArray
                ? addressArray.Select(a => a?.ToString() ?? "").ToList()
                : new(),

            PhoneNumber = firstItem["phone_number"]?.ToString(),
            Latitude = firstItem["latitude"]?.GetValue<double>() ?? 0,
            Longitude = firstItem["longitude"]?.GetValue<double>() ?? 0,
            Rating = firstItem["rating"]?.GetValue<double>() ?? 0,
            ReviewCount = firstItem["review_count"]?.GetValue<int>() ?? 0,
            Timezone = firstItem["timezone"]?.ToString(),
            Website = firstItem["website"]?.ToString(),
            PlaceId = firstItem["place_id"]?.ToString(),
            PlaceLink = firstItem["place_link"]?.ToString(),
            Types = firstItem["types"] is JsonArray typeArray
                ? typeArray.Select(t => t?.ToString() ?? "").ToList()
                : new(),

            PriceLevel = firstItem["price_level"]?.ToString(),
            PlusCode = firstItem["plus_code"]?.ToString(),
            Cid = firstItem["cid"]?.ToString(),
            IsClaimed = firstItem["is_claimed"]?.GetValue<bool>() ?? false,

            WorkingHours = firstItem["working_hours"] is JsonObject whObj
                ? whObj.ToDictionary(
                    wh => wh.Key,
                    wh => wh.Value?.AsArray()?.Select(v => v?.ToString() ?? "").ToList() ?? new()
                )
                : new(),

            City = firstItem["city"]?.ToString(),
            State = firstItem["state"]?.ToString(),

            Description = firstItem["description"] is JsonArray descArray
                ? descArray.Select(d => d?.ToString() ?? "").ToList()
                : new(),

            Details = firstItem["details"] is JsonObject detailsObj
                ? detailsObj.ToDictionary(
                    d => d.Key,
                    d => d.Value?.AsArray()?.Select(v => v?.ToString() ?? "").ToList() ?? new()
                )
                : new(),

            Photos = firstItem["photos"] is JsonArray photoArray
                ? photoArray.Select(p =>
                {
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
                : new()
        };

        return Result.Success(place);
    }
}