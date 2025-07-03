using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Application.Common.GeminiApi;
using Microsoft.Extensions.Logging;
using Mscc.GenerativeAI;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Prompt.Commands;

public sealed record CallGeminiStreamCommand(string PromptMessage) : ICommand<Result<GeminiResponse>>;

internal sealed class CallGeminiStreamHandler : ICommandHandler<CallGeminiStreamCommand, Result<GeminiResponse>>
{
    private readonly GenerativeModel _model;
    private readonly ILogger<CallGeminiStreamHandler> _logger;

    public CallGeminiStreamHandler(ILogger<CallGeminiStreamHandler> logger)
    {
        var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        var googleAi = new GoogleAI(apiKey);
        _model = googleAi.GenerativeModel(Model.Gemini25FlashPreview0520);
        _logger = logger;
    }

    public async Task<Result<Result<GeminiResponse>>> Handle(CallGeminiStreamCommand request,
        CancellationToken cancellationToken)
    {
        var prompt = PromptBuilder.BuildPrompt(request.PromptMessage);

        var fullResponse = new StringBuilder();

        await foreach (var chunk in _model.GenerateContentStream(prompt, null, null, null, null, null,
                           cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(chunk.Text))
            {
                _logger.LogInformation("🌱 Gemini stream chunk: {Chunk}", chunk.Text);
                fullResponse.Append(chunk.Text);
            }
        }


        var cleanedJson = CleanGeminiResponse.CleanResponse(fullResponse.ToString());

        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(cleanedJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });


        return Result.Success(geminiResponse);
    }
}