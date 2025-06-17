using System.Text;
using Application.Common.GeminiApi;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Prompt.Commands;

/// <summary>
/// Command gửi comment người dùng cho Gemini để đánh giá số sao
/// </summary>
/// <param name="UserComment">Nội dung comment</param>
public sealed record RateByGeminiCommand(string UserComment) : ICommand<float>;

internal sealed class RateByGeminiHandler : ICommandHandler<RateByGeminiCommand, float>
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string _model;

    public RateByGeminiHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        _model = "gemini-2.5-flash-preview-05-20";
    }

    public async Task<Result<float>> Handle(RateByGeminiCommand request, CancellationToken cancellationToken)
    {
        var prompt = PromptBuilder.BuildStarRatingPrompt(request.UserComment);

        var requestUrl =
            $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[] { new { text = prompt } }
                }
            }
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var responseText = JObject.Parse(responseString)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]
            ?.ToString();

        if (string.IsNullOrWhiteSpace(responseText))
            return Result.Failure<float>(Error.NullValue);

        try
        {
            responseText = CleanGeminiResponse.CleanResponse(responseText);

            var json = JObject.Parse(responseText);
            var stars = json["Stars"]?.Value<float>();

            if (stars is null)
                return Result.Failure<float>(Error.NullValue);

            return Result.Success(stars.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<float>(Error.FromException(ex));
        }
    }
}