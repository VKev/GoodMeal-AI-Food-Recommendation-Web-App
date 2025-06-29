using Application.Common.GeminiApi;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Prompt.Commands;

public sealed record GoogleImageSearchStreamCommand(List<string> FoodNames) : ICommand<Dictionary<string, string?>>;

public class
    GoogleImageSearchStreamHandler : ICommandHandler<GoogleImageSearchStreamCommand, Dictionary<string, string>>
{
    private readonly IDistributedCache _cache;
    private readonly IFoodImageRepository _foodImageRepository;
    private readonly HttpClient _httpClient;
    private readonly GoogleSearchBuilder _searchService;

    private readonly ILogger<GoogleImageSearchStreamHandler> _logger;

    public GoogleImageSearchStreamHandler(
        IDistributedCache cache,
        IFoodImageRepository foodImageRepository,
        HttpClient httpClient,
        GoogleSearchBuilder searchService,
        ILogger<GoogleImageSearchStreamHandler> logger)
    {
        _cache = cache;
        _foodImageRepository = foodImageRepository;
        _httpClient = httpClient;
        _searchService = searchService;
        _logger = logger;
    }

    //test stream response
    public async Task<Result<Dictionary<string, string>>> Handle(GoogleImageSearchStreamCommand request,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string>();

        foreach (var foodName in request.FoodNames)
        {
            _logger.LogInformation("🔍 Đang xử lý ảnh cho món: {FoodName}", foodName);

            var dbResult = await _foodImageRepository.GetByNameAsync(foodName, cancellationToken);
            if (dbResult != null && !string.IsNullOrWhiteSpace(dbResult.ImageUrl))
            {
                result[foodName] = dbResult.ImageUrl;

                _logger.LogInformation("✅ Lấy từ DB: {FoodName} - {ImageUrl}", foodName, dbResult.ImageUrl);
                continue;
            }

            var url = _searchService.BuildSearchUrl(foodName);
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ Google search fail cho món: {FoodName} - Status: {StatusCode}", foodName,
                    response.StatusCode);
                continue;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var imageUrl = _searchService.ExtractMostRelevantImageUrl(content, foodName);
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                _logger.LogWarning("❌ Không tìm thấy ảnh phù hợp cho món: {FoodName}", foodName);
                continue;
            }

            var foodImage = new FoodImage
            {
                Id = Guid.NewGuid(),
                FoodName = foodName,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };
            await _foodImageRepository.AddAsync(foodImage, cancellationToken);

            result[foodName] = imageUrl;

            var logMessage = new
            {
                FoodName = foodName,
                ImageUrl = imageUrl
            };
            _logger.LogInformation("📤 Response cho client: {Response}", logMessage);
        }

        return Result.Success(result);
    }
}