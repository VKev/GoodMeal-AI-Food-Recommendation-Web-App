using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using Application.Common.Dtos;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

public record SearchGeocodingAddressCommand(
    string Query,
    string Lang = "vi",
    string Country = "vn")
    : ICommand;

public class SearchGeocodingAddressCommandHandler : ICommandHandler<SearchGeocodingAddressCommand>
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SearchGeocodingAddressCommandHandler(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Result> Handle(SearchGeocodingAddressCommand request, CancellationToken cancellationToken)
    {
        var baseUrl = "https://maps-data.p.rapidapi.com";
        var apiKey = Environment.GetEnvironmentVariable("RAPID_API_KEY");
        var host = "maps-data.p.rapidapi.com";

        var url =
            $"{baseUrl}/geocoding.php?query={Uri.EscapeDataString(request.Query)}&country={request.Country}&lang={request.Lang}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Add("x-rapidapi-key", apiKey);
        httpRequest.Headers.Add("x-rapidapi-host", host);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"RapidAPI error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()} ");

        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        var firstItem = json?["data"];

        if (firstItem is null)
            return Result.Failure(Error.NullValue);

        var place = new GeocodingAddressDto
        {
            Address = firstItem["address"]?.ToString(),
            Latitude = firstItem["lat"]?.GetValue<double>() ?? 0,
            Longitude = firstItem["lng"]?.GetValue<double>() ?? 0,
            Timezone = firstItem["timezone"]?.ToString()
        };

        return Result.Success(place);
    }
}