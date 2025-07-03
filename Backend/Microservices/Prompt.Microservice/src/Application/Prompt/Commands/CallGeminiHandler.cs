using System.Text;
using Application.Common.GeminiApi;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Prompt.Commands;

public sealed record CallGeminiCommand(string PromptMessage) : ICommand<string>;

internal sealed class CallGeminiHandler : ICommandHandler<CallGeminiCommand, string>
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string _model;

    public CallGeminiHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        _model = "gemini-2.5-flash-preview-05-20";
    }

    public async Task<Result<string>> Handle(CallGeminiCommand request, CancellationToken cancellationToken)
    {
        var prompt = PromptBuilder.BuildPrompt(request.PromptMessage);

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

        var responseString = await response.Content.ReadAsStringAsync();

        var responseText = JObject.Parse(responseString)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]
            ?.ToString();
        if (string.IsNullOrWhiteSpace(responseText))
            throw new Exception("Gemini response is empty or invalid.");

        return Result.Success(responseText);
    }
}