using System.Diagnostics;
using Application.Common.GeminiApi;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Prompt.Commands;

public sealed record GoogleImageSearchCommand(List<string> FoodNames) : ICommand<Dictionary<string, string?>>;

internal sealed class GoogleImageSearchHandler : ICommandHandler<GoogleImageSearchCommand, Dictionary<string, string>>
{
    private readonly IDistributedCache _cache;
    private readonly IFoodImageRepository _foodImageRepository;
    private readonly HttpClient _httpClient;
    private readonly GoogleSearchBuilder _searchService;

    private readonly ILogger<GoogleImageSearchHandler> _logger;

    public GoogleImageSearchHandler(
        IDistributedCache cache,
        IFoodImageRepository foodImageRepository,
        HttpClient httpClient,
        GoogleSearchBuilder searchService,
        ILogger<GoogleImageSearchHandler> logger)
    {
        _cache = cache;
        _foodImageRepository = foodImageRepository;
        _httpClient = httpClient;
        _searchService = searchService;
        _logger = logger;
    }


    public async Task<Result<Dictionary<string, string>>> Handle(GoogleImageSearchCommand request,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string>();

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

        var missingNames = request.FoodNames.Except(result.Keys).ToList();
        var dbResults = await _foodImageRepository.GetByNamesAsync(missingNames, cancellationToken);

        foreach (var food in dbResults)
        {
            if (!string.IsNullOrWhiteSpace(food.ImageUrl))
            {
                result[food.FoodName] = food.ImageUrl;
            }
            // await _cache.SetStringAsync(food.FoodName, food.ImageUrl, new DistributedCacheEntryOptions
            // {
            //     AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            // }, cancellationToken);
        }

        var stillMissingNames = missingNames.Except(dbResults.Select(x => x.FoodName)).ToList();
        var stopwatch = Stopwatch.StartNew();
        var newFoodImages = new List<FoodImage>();
        await Parallel.ForEachAsync(stillMissingNames, new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (foodName, ct) =>
            {
                var url = _searchService.BuildSearchUrl(foodName);
                var response = await _httpClient.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode) return;

                var content = await response.Content.ReadAsStringAsync(ct);
                var imageUrl = _searchService.ExtractMostRelevantImageUrl(content, foodName);
                if (string.IsNullOrWhiteSpace(imageUrl)) return;

                var foodImage = new FoodImage
                {
                    Id = Guid.NewGuid(),
                    FoodName = foodName,
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                newFoodImages.Add(foodImage);
                result[foodName] = imageUrl;
                // await _cache.SetStringAsync(foodImage.FoodName, foodImage.ImageUrl, new DistributedCacheEntryOptions
                // {
                //     AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                // }, cancellationToken);
            });

        if (newFoodImages.Any())
        {
            await _foodImageRepository.AddRangeAsync(newFoodImages, cancellationToken);
        }

        stopwatch.Stop();
        _logger.LogInformation("Search & Save images for {Count} foods done in: {ElapsedMilliseconds} ms",
            stillMissingNames.Count, stopwatch.ElapsedMilliseconds);
        return Result.Success(result);
    }

}