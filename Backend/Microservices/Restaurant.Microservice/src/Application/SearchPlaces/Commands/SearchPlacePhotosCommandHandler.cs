using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Application.Common.Dtos;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

public record SearchPlacePhotosCommand(
    string Business_id,
    string Place_id,
    string Country = "vn",
    string Lang = "vi",
    string? cursor = null)
    : ICommand;

public class SearchPlacePhotosCommandHandler : ICommandHandler<SearchPlacePhotosCommand>
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SearchPlacePhotosCommandHandler(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Result> Handle(SearchPlacePhotosCommand request, CancellationToken cancellationToken)
    {
        var baseUrl = "https://maps-data.p.rapidapi.com";
        var apiKey = Environment.GetEnvironmentVariable("RAPID_API_KEY");
        var host = "maps-data.p.rapidapi.com";

        var url =
            $"{baseUrl}/photos.php?business_id={Uri.EscapeDataString(request.Business_id)}&country={request.Country}&lang={request.Lang}&cursor={request.cursor}&place_id={request.Place_id}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Add("x-rapidapi-key", apiKey);
        httpRequest.Headers.Add("x-rapidapi-host", host);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception(
                $"RapidAPI error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()} ");

        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        var photoUrls = json?["data"]?["photos"] as JsonArray;
        var photos = new List<PlacePhotoDto>();

        if (photoUrls != null)
        {
            foreach (var urlNode in photoUrls)
            {
                var urlPhoto = urlNode?.ToString();
                if (string.IsNullOrWhiteSpace(url))
                    continue;

                // Regex để lấy width/height
                var match = Regex.Match(urlPhoto, @"w(?<w>\d+)-h(?<h>\d+)");
                int maxWidth = match.Success ? int.Parse(match.Groups["w"].Value) : 0;
                int maxHeight = match.Success ? int.Parse(match.Groups["h"].Value) : 0;

                var photo = new PlacePhotoDto
                {
                    Src = urlPhoto,
                    MaxWidth = maxWidth,
                    MaxHeight = maxHeight,
                    MinWidth = 0, // hoặc bạn có thể tính thêm nếu cần
                    MinHeight = 0
                };

                photos.Add(photo);
            }
        }

        return Result.Success(photos);
    }
}