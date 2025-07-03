using Application.Common.GeminiApi;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Prompt.Commands;

public sealed record GoogleImageSearchWithVisionApiCommand(List<string> FoodNames)
    : ICommand<Dictionary<string, string?>>;

internal sealed class
    GoogleImageSearchWithVisionApiHandler : ICommandHandler<GoogleImageSearchWithVisionApiCommand,
    Dictionary<string, string>>
{
    private readonly IDistributedCache _cache;
    private readonly IFoodImageRepository _foodImageRepository;
    private readonly HttpClient _httpClient;
    private readonly GoogleSearchBuilder _searchService;
    private readonly IMediator _mediator;

    public GoogleImageSearchWithVisionApiHandler(
        IDistributedCache cache,
        IFoodImageRepository foodImageRepository,
        HttpClient httpClient,
        GoogleSearchBuilder searchService,
        IMediator mediator
    )
    {
        _cache = cache;
        _foodImageRepository = foodImageRepository;
        _httpClient = httpClient;
        _searchService = searchService;
        _mediator = mediator;
    }

    public async Task<Result<Dictionary<string, string>>> Handle(GoogleImageSearchWithVisionApiCommand request,
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
            result[food.FoodName] = food.ImageUrl;

            // await _cache.SetStringAsync(food.FoodName, food.ImageUrl, new DistributedCacheEntryOptions
            // {
            //     AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            // }, cancellationToken);
        }

        var stillMissingNames = missingNames.Except(dbResults.Select(x => x.FoodName)).ToList();

        await Parallel.ForEachAsync(stillMissingNames, new ParallelOptions { MaxDegreeOfParallelism = 10 },
            async (foodName, ct) =>
            {
                var url = _searchService.BuildSearchUrl(foodName);
                var response = await _httpClient.GetAsync(url, ct);
                if (!response.IsSuccessStatusCode) return;

                var content = await response.Content.ReadAsStringAsync(ct);
                var imageUrls = _searchService.ExtractMostRelevantImageUrl(content,foodName);
                if (!imageUrls.Any()) return;
                foreach (var imageUrl in imageUrls)
                {
                    // var labelsResult = await _mediator.Send(new DetectImageLabelsCommand(imageUrl), ct);
                    // if (!labelsResult.IsSuccess) continue;
                    //
                    // var labels = labelsResult.Value;
                    // var foodNameNoDiacritics = StringExtensions.RemoveDiacritics(foodName).ToLower();
                    //
                    //
                    // if (labels.Any(l => l.RemoveDiacritics().ToLower().Contains(foodNameNoDiacritics)))
                    // {
                    //     var foodImage = new FoodImage
                    //     {
                    //         Id = Guid.NewGuid(),
                    //         FoodName = foodName,
                    //         ImageUrl = imageUrl,
                    //         CreatedAt = DateTime.UtcNow
                    //     };
                    //
                    //     await _foodImageRepository.AddAsync(foodImage, ct);
                    //     result[foodName] = imageUrl;
                    //     break;
                    // }
                }
            });


        return Result.Success(result);
    }
}