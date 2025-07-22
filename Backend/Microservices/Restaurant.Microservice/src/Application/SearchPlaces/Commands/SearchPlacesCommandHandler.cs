using System.Text.Json.Nodes;
using Domain.Common.Dtos;
using RestSharp;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.SearchPlaces.Commands;

public sealed record SearchPlacesCommand(string Query, double Latitude, double Longitude)
    : ICommand;

internal sealed class SearchPlacesCommandHandler: ICommandHandler<SearchPlacesCommand>
{

    public async Task<Result> Handle(SearchPlacesCommand request, CancellationToken cancellationToken)
    {
        var apiKey = Environment.GetEnvironmentVariable("FOURSQUARE_API_KEY");
        var baseUrl = "https://api.foursquare.com/v3/places";

        var options = new RestClientOptions($"{baseUrl}/search");
        var client = new RestClient(options);
        var restRequest = new RestRequest()
            .AddHeader("accept", "application/json")
            .AddHeader("Authorization", apiKey)
            .AddQueryParameter("query", request.Query)
            .AddQueryParameter("ll", $"{request.Latitude},{request.Longitude}")
            .AddQueryParameter("radius", "3000")
            .AddQueryParameter("limit", "10");

        var response = await client.GetAsync(restRequest, cancellationToken);

        if (!response.IsSuccessful)
            throw new Exception($"Foursquare API error: {response.StatusCode} - {response.Content}");

        var json = JsonNode.Parse(response.Content);
        var results = json?["results"]?.AsArray();
        
        var restaurantResponses = results?.Select(item => new PlaceDto
        {
            FsqId = item?["fsq_id"]?.ToString(),
            Name = item?["name"]?.ToString(),
            Category = item?["categories"]?.AsArray()?.FirstOrDefault()?["name"]?.ToString() ?? "Unknown",
            Address = item?["location"]?["formatted_address"]?.ToString(),
            Latitude = item?["geocodes"]?["main"]?["latitude"]?.GetValue<double>() ?? 0,
            Longitude = item?["geocodes"]?["main"]?["longitude"]?.GetValue<double>() ?? 0,
            Distance = item?["distance"]?.GetValue<int>() ?? -1
        }).ToList() ?? new(); 
        return Result.Success(restaurantResponses);
    }
}