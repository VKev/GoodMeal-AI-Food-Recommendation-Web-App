using System.Security.Claims;
using System.Text;
using Application.Common.GeminiApi;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedLibrary.Common;

namespace Application.Prompt.Commands;

public sealed record UpdateMessageCommand(
    Guid Id,
    string PromptMessage,
    string ResponseMessage
) : ICommand;

internal sealed class UpdateMessageHandler : ICommandHandler<UpdateMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string _model;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateMessageHandler(
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor)
    {
        _messageRepository = messageRepository;
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        _model = "gemini-2.5-flash-preview-05-20";
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
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
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Gemini API failed: {response.StatusCode}");

        var responseString = await response.Content.ReadAsStringAsync();
        var responseText = JObject.Parse(responseString)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]
            ?.ToString();

        if (string.IsNullOrWhiteSpace(responseText))
            throw new Exception("Gemini response is empty or invalid.");

        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            return Result.Failure(new Error("Auth.Unauthoried", "User is not authenticated"));
        }

        var userId = userIdClaim.Value;
        var updatedMessage = new Message
        {
            Id = request.Id,
            PromptMessage = request.PromptMessage,
            ResponseMessage = JsonConvert.SerializeObject(responseText),
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        _messageRepository.UpdateFields(updatedMessage,
            x => x.PromptMessage,
            x => x.ResponseMessage,
            x => x.UpdatedAt);

        return Result.Success();
    }
}