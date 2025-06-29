using System.Diagnostics;
using System.Text.Json;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Prompt.Commands;

public sealed record RapidApiImageSearchCommand(List<string> FoodNames) : ICommand<Dictionary<string, string?>>;

public class RapidApiSearchHandler : ICommandHandler<RapidApiImageSearchCommand, Dictionary<string, string?>>
{
    private readonly IDistributedCache _cache;
    private readonly IFoodImageRepository _foodImageRepository;
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string? _host;

    public RapidApiSearchHandler(
        IDistributedCache cache,
        IFoodImageRepository foodImageRepository,
        HttpClient httpClient)
    {
        _cache = cache;
        _foodImageRepository = foodImageRepository;
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("RAPIDAPI_KEY");
        _host = Environment.GetEnvironmentVariable("RAPIDAPI_HOST");
    }

    public async Task<Result<Dictionary<string, string?>>> Handle(RapidApiImageSearchCommand request,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string?>();

        // var cacheTasks = request.FoodNames.Select(name => _cache.GetStringAsync(name, cancellationToken));
        // var cachedResults = await Task.WhenAll(cacheTasks);
        //
        // var cacheMap = request.FoodNames
        //     .Zip(cachedResults, (name, image) => new { name, image })
        //     .Where(x => !string.IsNullOrEmpty(x.image))
        //     .ToDictionary(x => x.name, x => x.image!);
        //
        // foreach (var kvp in cacheMap)
        // {
        //     result[kvp.Key] = kvp.Value;
        // }

        var remainingNames = request.FoodNames.Except(result.Keys).ToList();

        if (remainingNames.Any())
        {
            var dbResults = await _foodImageRepository.GetByNamesAsync(remainingNames, cancellationToken);
            foreach (var food in dbResults)
            {
                result[food.FoodName] = food.ImageUrl;

                // await _cache.SetStringAsync(food.FoodName, food.ImageUrl, new DistributedCacheEntryOptions
                // {
                //     AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                // }, cancellationToken);
            }

            remainingNames = remainingNames.Except(dbResults.Select(x => x.FoodName)).ToList();
        }

        await Parallel.ForEachAsync(remainingNames, new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (foodName, ct) =>
            {
                try
                {
                    var apiUrl =
                        $"https://google-search-master-mega.p.rapidapi.com/images?q={Uri.EscapeDataString(foodName)}&gl=vn&hl=vn&autocorrect=true&num=1&page=1";

                    var httpRequest = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(apiUrl),
                        Headers =
                        {
                            { "x-rapidapi-key", _apiKey },
                            { "x-rapidapi-host", _host },
                        },
                    };
                    var stopwatch = Stopwatch.StartNew();
                    using var response = await _httpClient.SendAsync(httpRequest, ct);
                    if (!response.IsSuccessStatusCode) return;
                    stopwatch.Stop();
                    Console.WriteLine($"[RapidAPI] Request for '{foodName}' took {stopwatch.ElapsedMilliseconds} ms");
                    var content = await response.Content.ReadAsStringAsync(ct);

                    var searchResult = JsonSerializer.Deserialize<ImageSearchResponse>(content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    var imageUrl = searchResult?.Images.FirstOrDefault()?.ImageUrl;
                    if (string.IsNullOrWhiteSpace(imageUrl)) return;

                    var foodImage = new FoodImage
                    {
                        Id = Guid.NewGuid(),
                        FoodName = foodName,
                        ImageUrl = imageUrl,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _foodImageRepository.AddAsync(foodImage, ct);
                    result[foodName] = imageUrl;

                    // await _cache.SetStringAsync(foodName, imageUrl, new DistributedCacheEntryOptions
                    // {
                    //     AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                    // }, ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

        return Result.Success(result);
    }
}