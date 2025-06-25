using System.Text.Json;
using Application.Common.GeminiApi;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Prompt.Commands;

public sealed record GoogleImageSearchCommand(List<string> FoodNames) : ICommand<Dictionary<string, string?>>;

internal sealed class GoogleImageSearchHandler : ICommandHandler<GoogleImageSearchCommand, Dictionary<string, string?>>
{
    private readonly HttpClient _httpClient;
    private readonly GoogleSearchBuilder _searchService;

    public GoogleImageSearchHandler(HttpClient httpClient, GoogleSearchBuilder searchService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    public async Task<Result<Dictionary<string, string?>>> Handle(GoogleImageSearchCommand request,
        CancellationToken cancellationToken)
    {
        var semaphore = new SemaphoreSlim(10); 
        var tasks = request.FoodNames.Select(async foodName =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var url = _searchService.BuildSearchUrl(foodName);
                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Handle] Failed request for {foodName}, Status: {response.StatusCode}");
                    return new KeyValuePair<string, string?>(foodName, null);
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var imageUrl = _searchService.ExtractImageUrl(content);
                
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    var fallbackUrl = _searchService.BuildSearchUrl(foodName);
                    var fallbackResponse = await _httpClient.GetAsync(fallbackUrl, cancellationToken);

                    if (fallbackResponse.IsSuccessStatusCode)
                    {
                        var fallbackContent = await fallbackResponse.Content.ReadAsStringAsync(cancellationToken);
                        imageUrl = _searchService.ExtractImageUrl(fallbackContent);
                    }
                }

                return new KeyValuePair<string, string?>(foodName, imageUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Handle] Error processing {foodName}: {ex.Message}");
                return new KeyValuePair<string, string?>(foodName, null);
            }
            finally
            {
                semaphore.Release();
            }
        });

        try
        {
            var resultsArray = await Task.WhenAll(tasks);
            var results = resultsArray.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return Result.Success(results);
        }
        catch (Exception ex)
        {
            return Result.Failure<Dictionary<string, string?>>(Error.FromException(ex));
        }
    }
}